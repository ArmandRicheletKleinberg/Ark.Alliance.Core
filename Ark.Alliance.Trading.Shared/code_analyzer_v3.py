#!/usr/bin/env python3
"""
Code Structure and Content Analyzer (v3)
Generates a comprehensive MD file with directory structure and code file contents.
- Fine-grained Markdown inclusion (none/readme/all)
- Stricter default exclusions for CSS, XML, TypeScript, Python, and specific directories
- Optional exclusion of JavaScript/JSX sources (.js/.jsx) — controllable via CLI or interactive menu

Author: Updated by ChatGPT
"""

import os
import re
import argparse
from pathlib import Path
from datetime import datetime
from typing import List, Dict, Tuple, Set, Optional

class CodeAnalyzer:
    def __init__(
        self,
        root_path: str,
        output_file: str = "code_structure_analysis.md",
        remove_comments: bool = False,
        md_mode: str = "readme",  # 'none' | 'readme' | 'all'
        include_js: bool = True,  # include .js/.jsx/.jsxc
        interactive_menu: bool = False
    ):
        """
        Parameters
        ----------
        root_path : str
            The root directory to analyze.
        output_file : str
            The output markdown file path.
        remove_comments : bool
            Whether to remove comments from supported code files.
        md_mode : str
            Markdown inclusion mode:
              - 'none'   : exclude all *.md files
              - 'readme' : include only README.md files (default)
              - 'all'    : include all *.md files
        include_js : bool
            Whether to include JavaScript/JSX files (.js/.jsx/.jsxc). Default: True.
        interactive_menu : bool
            If True, present a full menu to set all options when generating the output.
        """
        self.root_path = Path(root_path).resolve()
        self.output_file = Path(output_file).resolve()
        self.remove_comments = remove_comments
        self.md_mode = md_mode.lower()
        self.include_js = include_js
        self._interactive_menu = interactive_menu

        # Supported code file extensions (explicitly excluding: .css, .xml, .py, .ts, .tsx)
        # Include only these:
        self.code_extensions = {
            ".sln", ".csproj", ".cs", ".js", ".jsx", ".jsxc", ".razor", ".json", ".md"
        }

        # File type mappings for syntax highlighting
        self.syntax_mapping = {
            ".sln": "text",
            ".csproj": "xml",
            ".cs": "csharp",
            ".js": "javascript",
            ".jsx": "jsx",
            ".jsxc": "jsx",
            ".razor": "html",
            ".json": "json",
            ".md": "markdown",
        }

    def is_code_file(self, file_path: Path) -> bool:
        """Check if file is a code/markdown file we want to analyze"""
        ext = file_path.suffix.lower()
        if ext not in self.code_extensions:
            return False

        # Handle Markdown inclusion policy
        if ext == ".md":
            if self.md_mode == "none":
                return False
            if self.md_mode == "readme" and file_path.name.lower() != "readme.md":
                return False

        # Optional exclusion for JavaScript/JSX
        if ext in {".js", ".jsx", ".jsxc"} and not self.include_js:
            return False

        return True

    def should_exclude_file(self, file_path: Path) -> bool:
        """Check if file or any of its parent folders should be excluded"""
        exclude_patterns = {
            # Build/Output directories
            "bin", "obj", "dist", "build", "out", "target",
            # Dependencies
            "node_modules", "packages", "__pycache__", ".pytest_cache",
            # IDE/Editor files
            ".vs", ".vscode", ".idea",
            # Version control
            ".git", ".svn", ".hg", ".github",
            # Web roots and misc
            "wwwroot",
            # Other
            ".azure", ".env", "logs",
        }

        # If any part of the path matches excluded directories, skip
        path_parts = set(file_path.parts)
        if path_parts.intersection(exclude_patterns):
            return True

        # Exclude certain filenames/patterns
        exclude_file_patterns = {
            ".min.js", ".min.css", "bundle.js", "bundle.css",
            "package-lock.json", "yarn.lock", "pnpm-lock.yaml",
        }
        name_lower = file_path.name.lower()
        for pattern in exclude_file_patterns:
            if pattern in name_lower:
                return True

        # Exclude CSS and plain XML files outright (even if added later)
        if file_path.suffix.lower() in {".css", ".xml"}:
            return True

        # .py and .ts/.tsx are excluded by design (not added to self.code_extensions)

        return False

    def _ask_yes_no(self, prompt: str, default: Optional[bool] = None) -> bool:
        """Prompt the user for a yes/no answer in interactive mode"""
        suffix = " [y/n]"
        if default is True:
            suffix = " [Y/n]"
        elif default is False:
            suffix = " [y/N]"
        while True:
            ans = input(f"{prompt}{suffix}: ").strip().lower()
            if ans in {"y", "yes"}:
                return True
            if ans in {"n", "no"}:
                return False
            if ans == "" and default is not None:
                return default
            print("Please answer 'y' or 'n'.")

    def _ask_choice(self, prompt: str, choices: Dict[str, str], default_key: Optional[str] = None) -> str:
        """Prompt the user to choose one option among dict keys (like 1/2/3)"""
        # Display menu
        print(prompt)
        for k, label in choices.items():
            default_mark = " (default)" if default_key is not None and k == default_key else ""
            print(f"  {k}) {label}{default_mark}")
        while True:
            ans = input("Select: ").strip()
            if ans in choices:
                return ans
            if ans == "" and default_key is not None:
                return default_key
            print("Invalid choice. Please select a valid option.")

    def run_interactive_menu(self):
        """Present a full menu to set all options if interactive mode is enabled"""
        print("\n🛠  Interactive options menu\n")

        # 1) Comments
        self.remove_comments = self._ask_yes_no("Remove comments/documentation from code?", default=self.remove_comments)

        # 2) Markdown mode
        md_choices = {
            "1": "Include only README.md files",
            "2": "Include all *.md files",
            "3": "Exclude all Markdown files",
        }
        md_map = {"1": "readme", "2": "all", "3": "none"}
        md_default_key = {"readme": "1", "all": "2", "none": "3"}.get(self.md_mode, "1")
        md_sel = self._ask_choice("Markdown inclusion mode:", md_choices, default_key=md_default_key)
        self.md_mode = md_map[md_sel]

        # 3) Include JS/JSX
        self.include_js = self._ask_yes_no("Include JavaScript/JSX files (.js/.jsx)?", default=self.include_js)

        print("")

    def remove_comments_from_content(self, content: str, file_extension: str) -> str:
        """Remove comments from code content based on file type"""
        if not self.remove_comments:
            return content

        if file_extension in [".cs", ".razor"]:
            return self._remove_csharp_comments(content)
        elif file_extension in [".js", ".jsx", ".jsxc"]:
            return self._remove_javascript_comments(content)
        elif file_extension == ".json":
            return self._remove_json_comments(content)
        elif file_extension in [".sln", ".csproj"]:
            return self._remove_xml_comments(content)
        elif file_extension == ".md":
            # No comment removal for Markdown; return as-is
            return content
        else:
            return content

    def _remove_csharp_comments(self, content: str) -> str:
        """Remove C# comments including XML documentation"""
        lines = content.split("\n")
        result_lines = []
        in_multiline_comment = False

        for line in lines:
            original_line = line
            line = line.rstrip()

            if in_multiline_comment:
                # Look for end of multiline comment
                end_pos = line.find("*/")
                if end_pos != -1:
                    line = line[end_pos + 2 :]
                    in_multiline_comment = False
                else:
                    continue

            # Remove XML documentation comments (///)
            if line.strip().startswith("///"):
                continue

            # Remove single line comments (//) without touching strings
            in_string = False
            escaped = False
            comment_pos = -1

            for i, char in enumerate(line):
                if escaped:
                    escaped = False
                    continue

                if char == "\\":
                    escaped = True
                    continue

                if char == '"' and not escaped:
                    in_string = not in_string
                    continue

                if not in_string and i < len(line) - 1:
                    if line[i : i + 2] == "//":
                        comment_pos = i
                        break
                    elif line[i : i + 2] == "/*":
                        # Start of multiline comment
                        end_pos = line.find("*/", i + 2)
                        if end_pos != -1:
                            # Comment ends on same line
                            line = line[:i] + line[end_pos + 2 :]
                            break
                        else:
                            # Comment continues to next line
                            line = line[:i]
                            in_multiline_comment = True
                            break

            if comment_pos != -1:
                line = line[:comment_pos].rstrip()

            if line.strip() or not original_line.strip():
                result_lines.append(line)

        return "\n".join(result_lines)

    def _remove_javascript_comments(self, content: str) -> str:
        """Remove JavaScript/JSX comments"""
        lines = content.split("\n")
        result_lines = []
        in_multiline_comment = False

        for line in lines:
            original_line = line
            line = line.rstrip()

            if in_multiline_comment:
                end_pos = line.find("*/")
                if end_pos != -1:
                    line = line[end_pos + 2 :]
                    in_multiline_comment = False
                else:
                    continue

            # Preserve strings while removing comments
            in_string = False
            escaped = False
            string_char = None
            comment_pos = -1

            i = 0
            while i < len(line):
                char = line[i]

                if escaped:
                    escaped = False
                    i += 1
                    continue

                if char == "\\":
                    escaped = True
                    i += 1
                    continue

                if not in_string:
                    if char in ['"', "'", "`"]:
                        in_string = True
                        string_char = char
                    elif char == "/" and i < len(line) - 1:
                        next_char = line[i + 1]
                        if next_char == "/":
                            comment_pos = i
                            break
                        elif next_char == "*":
                            end_pos = line.find("*/", i + 2)
                            if end_pos != -1:
                                line = line[:i] + line[end_pos + 2 :]
                                continue
                            else:
                                line = line[:i]
                                in_multiline_comment = True
                                break
                elif in_string and char == string_char:
                    in_string = False
                    string_char = None

                i += 1

            if comment_pos != -1:
                line = line[:comment_pos].rstrip()

            if line.strip() or not original_line.strip():
                result_lines.append(line)

        return "\n".join(result_lines)

    def _remove_json_comments(self, content: str) -> str:
        """Remove // comments from JSON-like files (non-standard)"""
        lines = content.split("\n")
        result_lines = []

        for line in lines:
            original_line = line
            comment_pos = line.find("//")
            if comment_pos != -1:
                # Ensure not inside a string
                quote_count = line[:comment_pos].count('"')
                if quote_count % 2 == 0:
                    line = line[:comment_pos].rstrip()

            if line.strip() or not original_line.strip():
                result_lines.append(line)

        return "\n".join(result_lines)

    def _remove_xml_comments(self, content: str) -> str:
        """Remove XML comments from .csproj/.sln"""
        content = re.sub(r"<!--.*?-->", "", content, flags=re.DOTALL)
        # Clean up empty lines
        lines = content.split("\n")
        result_lines = []
        for line in lines:
            if line.strip() or not line:
                result_lines.append(line.rstrip())
        return "\n".join(result_lines)

    def generate_directory_tree(self) -> List[str]:
        """Generate a visual directory tree structure for included files only"""
        tree_lines = []

        def add_directory_tree(directory: Path, prefix: str = "", is_last: bool = True):
            if directory.name.startswith(".") or self.should_exclude_file(directory):
                return

            # Check if directory contains any included files
            has_included_files = any(
                self.is_code_file(f) and not self.should_exclude_file(f)
                for f in directory.rglob("*") if f.is_file()
            )
            if not has_included_files:
                return

            connector = "└── " if is_last else "├── "
            tree_lines.append(f"{prefix}{connector}{directory.name}/")

            try:
                items = list(directory.iterdir())
                items = [item for item in items if not item.name.startswith(".") and not self.should_exclude_file(item)]
                items.sort(key=lambda x: (x.is_file(), x.name.lower()))

                directories = [item for item in items if item.is_dir()]
                files = [item for item in items if item.is_file() and self.is_code_file(item)]

                # Filter to directories that contain included files
                code_directories = []
                for subdir in directories:
                    if any(self.is_code_file(f) and not self.should_exclude_file(f)
                           for f in subdir.rglob("*") if f.is_file()):
                        code_directories.append(subdir)

                # Add directories
                for i, subdir in enumerate(code_directories):
                    is_last_dir = (i == len(code_directories) - 1) and len(files) == 0
                    new_prefix = prefix + ("    " if is_last else "│   ")
                    add_directory_tree(subdir, new_prefix, is_last_dir)

                # Add files
                for i, file in enumerate(files):
                    is_last_file = i == len(files) - 1
                    file_connector = "└── " if is_last_file else "├── "
                    new_prefix = prefix + ("    " if is_last else "│   ")

                    try:
                        size = file.stat().st_size
                        size_str = self.format_file_size(size)
                        tree_lines.append(f"{new_prefix}{file_connector}{file.name} ({size_str})")
                    except Exception:
                        tree_lines.append(f"{new_prefix}{file_connector}{file.name}")

            except PermissionError:
                tree_lines.append(f"{prefix}    [Permission Denied]")

        # Start with root
        tree_lines.append(f"{self.root_path.name}/")

        try:
            items = list(self.root_path.iterdir())
            items = [item for item in items if not item.name.startswith(".") and not self.should_exclude_file(item)]
            items.sort(key=lambda x: (x.is_file(), x.name.lower()))

            directories = [item for item in items if item.is_dir()]
            files = [item for item in items if item.is_file() and self.is_code_file(item)]

            # Filter directories
            code_directories = []
            for subdir in directories:
                if any(self.is_code_file(f) and not self.should_exclude_file(f)
                       for f in subdir.rglob("*") if f.is_file()):
                    code_directories.append(subdir)

            # Process directories
            for i, subdir in enumerate(code_directories):
                is_last_dir = (i == len(code_directories) - 1) and len(files) == 0
                add_directory_tree(subdir, "", is_last_dir)

            # Process root files
            for i, file in enumerate(files):
                is_last_file = i == len(files) - 1
                connector = "└── " if is_last_file else "├── "
                try:
                    size = file.stat().st_size
                    size_str = self.format_file_size(size)
                    tree_lines.append(f"{connector}{file.name} ({size_str})")
                except Exception:
                    tree_lines.append(f"{connector}{file.name}")

        except PermissionError:
            tree_lines.append("[Permission Denied]")

        return tree_lines

    def format_file_size(self, size_bytes: int) -> str:
        """Format file size in human-readable units"""
        if size_bytes == 0:
            return "0 B"
        size_names = ["B", "KB", "MB", "GB"]
        i = 0
        size = float(size_bytes)
        while size >= 1024 and i < len(size_names) - 1:
            size /= 1024.0
            i += 1
        return f"{int(size)} {size_names[i]}" if i == 0 else f"{size:.1f} {size_names[i]}"

    def find_code_files(self) -> Dict[str, List[Path]]:
        """Find all included files organized by directory"""
        code_files_by_dir: Dict[str, List[Path]] = {}

        for root, dirs, files in os.walk(self.root_path):
            # Skip excluded directories
            dirs[:] = [d for d in dirs if not d.startswith(".") and not self.should_exclude_file(Path(root) / d)]

            root_path = Path(root)
            if self.should_exclude_file(root_path):
                continue

            relative_path = root_path.relative_to(self.root_path)

            code_files_in_dir = []
            for file in files:
                if file.startswith("."):
                    continue
                file_path = root_path / file
                if self.is_code_file(file_path) and not self.should_exclude_file(file_path):
                    code_files_in_dir.append(file_path)

            if code_files_in_dir:
                code_files_in_dir.sort(key=lambda x: x.name.lower())
                code_files_by_dir[str(relative_path)] = code_files_in_dir

        return code_files_by_dir

    def read_code_file(self, file_path: Path) -> str:
        """Read file content safely (with optional comment stripping)"""
        try:
            with open(file_path, "r", encoding="utf-8") as f:
                content = f.read()
        except UnicodeDecodeError:
            try:
                with open(file_path, "r", encoding="latin-1") as f:
                    content = f.read()
            except Exception as e:
                return f"Error reading file: {str(e)}"
        except Exception as e:
            return f"Error reading file: {str(e)}"

        # Remove comments if requested
        return self.remove_comments_from_content(content, file_path.suffix.lower())

    def generate_statistics(self, code_files_by_dir: Dict[str, List[Path]]) -> Dict:
        """Generate simple project statistics"""
        stats = {
            "total_directories": len(code_files_by_dir),
            "total_files": 0,
            "total_size": 0,
            "file_types": {},
        }

        for dir_files in code_files_by_dir.values():
            stats["total_files"] += len(dir_files)
            for file_path in dir_files:
                try:
                    stats["total_size"] += file_path.stat().st_size
                    ext = file_path.suffix.lower()
                    stats["file_types"][ext] = stats["file_types"].get(ext, 0) + 1
                except Exception:
                    pass

        return stats

    def get_syntax_highlight(self, file_path: Path) -> str:
        """Get syntax highlighting language for file code block"""
        return self.syntax_mapping.get(file_path.suffix.lower(), "text")

    def generate_output_file(self):
        """Generate the comprehensive output markdown file"""
        print(f"🔍 Analyzing: {self.root_path}")

        # Interactive full menu (only if requested by main when args absent)
        if getattr(self, "_interactive_menu", False):
            self.run_interactive_menu()

        # If remove_comments wasn't specified via CLI and no interactive menu, ask once
        if not getattr(self, "_interactive_menu", False) and not hasattr(self, "_comment_choice_made"):
            self.remove_comments = self._ask_yes_no("\n❓ Remove comments/documentation from code?", default=self.remove_comments)

        tree_structure = self.generate_directory_tree()
        code_files_by_dir = self.find_code_files()
        stats = self.generate_statistics(code_files_by_dir)

        print("✍️ Generating output...")
        output_lines: List[str] = []

        # Header
        comment_status = "Comments Removed" if self.remove_comments else "Comments Preserved"
        md_status = {
            "none": "No Markdown files",
            "readme": "README.md only",
            "all": "All Markdown files",
        }.get(self.md_mode, "README.md only")
        js_status = "Included" if self.include_js else "Excluded"

        output_lines.extend(
            [
                "# Code Structure Analysis",
                "",
                f"**Generated on:** {datetime.now().strftime('%Y-%m-%d %H:%M:%S')}",
                f"**Root Directory:** `{self.root_path}`",
                f"**Comment Processing:** {comment_status}",
                f"**Markdown Included:** {md_status}",
                f"**JavaScript/JSX:** {js_status}",
                f"**Analysis Tool:** Code Structure and Content Analyzer (v3)",
                "",
                "---",
                "",
            ]
        )

        # Statistics
        output_lines.extend(
            [
                "## 📊 Project Statistics",
                "",
                f"- **Total Directories (with files):** {stats['total_directories']}",
                f"- **Total Included Files:** {stats['total_files']}",
                f"- **Total Size:** {self.format_file_size(stats['total_size'])}",
                "",
            ]
        )

        # File type breakdown
        if stats["file_types"]:
            output_lines.extend(["### File Type Breakdown", ""])
            for ext, count in sorted(stats["file_types"].items()):
                output_lines.append(f"- **{ext}**: {count} files")
            output_lines.extend(["", "---", ""])

        # Directory structure
        output_lines.extend(
            [
                "## 📁 Directory Structure (Included Files)",
                "",
                "```",
                *tree_structure,
                "```",
                "",
                "---",
                "",
            ]
        )

        # File contents
        output_lines.extend(
            [
                "## 💻 File Contents",
                "",
                f"Complete content of all included files {'(comments removed)' if self.remove_comments else '(with comments)'}:",
                "",
            ]
        )

        sorted_dirs = sorted(code_files_by_dir.keys())
        for i, dir_path in enumerate(sorted_dirs):
            files = code_files_by_dir[dir_path]

            # Directory header
            if dir_path == ".":
                display_path = "Root Directory"
                full_path = str(self.root_path)
            else:
                display_path = f"Directory: {dir_path}"
                full_path = str(self.root_path / dir_path)

            output_lines.extend([f"### {display_path}", "", f"**Full Path:** `{full_path}`", f"**Files:** {len(files)}", ""])

            for j, file in enumerate(files):
                file_path = file
                relative_file_path = file_path.relative_to(self.root_path)
                syntax = self.get_syntax_highlight(file_path)

                output_lines.extend(
                    [
                        f"#### 💾 {file_path.name}",
                        "",
                        f"**File Path:** `{relative_file_path}`",
                        f"**File Type:** {file_path.suffix.upper()} ({syntax})",
                        "",
                    ]
                )

                # Read and include file content
                content = self.read_code_file(file_path)
                output_lines.extend([f"```{syntax}", content, "```", ""])

                # Add separator between files
                if not (i == len(sorted_dirs) - 1 and j == len(files) - 1):
                    output_lines.extend(["---", ""])

        # Footer
        output_lines.extend(
            [
                "",
                "---",
                "",
                f"**Analysis completed on {datetime.now().strftime('%Y-%m-%d at %H:%M:%S')}**",
                f"**Comments:** {'Removed for clean documentation' if self.remove_comments else 'Preserved as written'}",
                f"**JavaScript/JSX:** {'Included' if self.include_js else 'Excluded'}",
                "",
                "*Generated by Code Structure and Content Analyzer (v3)*",
            ]
        )

        # Write to output file
        print(f"💾 Writing to: {self.output_file}")
        try:
            with open(self.output_file, "w", encoding="utf-8") as f:
                f.write("\n".join(output_lines))

            print("✅ Analysis complete!")
            print(f"📄 Output file: {self.output_file}")
            print(f"💻 Processed {stats['total_files']} files from {len(code_files_by_dir)} directories")

        except Exception as e:
            print(f"❌ Error writing output file: {str(e)}")
            return False

        return True

def main():
    """Main function with command line argument parsing"""
    parser = argparse.ArgumentParser(
        description="Analyze project code structure and consolidate code files into Markdown",
        formatter_class=argparse.RawDescriptionHelpFormatter,
        epilog=r"""
Supported file types (default filters applied):
  - Included: .sln, .csproj, .cs, .js, .jsx, .jsxc, .razor, .json, .md
  - Excluded by design: .css, .xml, .py, .ts, .tsx
  - Excluded by path: bin/, obj/, .vs/, .github/, wwwroot/, node_modules/, etc.

Examples:
  python code_analyzer_v3.py                              # Full interactive menu (no options provided)
  python code_analyzer_v3.py . -o out.md --keep-comments  # Keep comments (no menu)
  python code_analyzer_v3.py . --remove-comments          # Strip comments from supported languages
  python code_analyzer_v3.py /path --md none              # Exclude all Markdown
  python code_analyzer_v3.py /path --md all               # Include all Markdown
  python code_analyzer_v3.py /path --exclude-js           # Exclude .js/.jsx/.jsxc files
""",
    )

    parser.add_argument(
        "directory",
        nargs="?",
        default=".",
        help="Directory to analyze (default: current directory)",
    )

    parser.add_argument(
        "-o",
        "--output",
        default="code_structure_analysis.md",
        help="Output file name (default: code_structure_analysis.md)",
    )

    comment_group = parser.add_mutually_exclusive_group()
    comment_group.add_argument(
        "--remove-comments",
        action="store_true",
        help="Remove all comments and documentation from code",
    )
    comment_group.add_argument(
        "--keep-comments",
        action="store_true",
        help="Keep all comments and documentation in code",
    )

    parser.add_argument(
        "--md",
        choices=["none", "readme", "all"],
        default="readme",
        help="Markdown inclusion policy: 'none' (exclude all), 'readme' (include README.md only) [default], or 'all' (include all *.md)",
    )

    js_group = parser.add_mutually_exclusive_group()
    js_group.add_argument(
        "--exclude-js",
        action="store_true",
        help="Exclude JavaScript/JSX files (.js/.jsx/.jsxc)",
    )
    js_group.add_argument(
        "--include-js",
        action="store_true",
        help="Include JavaScript/JSX files (.js/.jsx/.jsxc) [default]",
    )

    args = parser.parse_args()

    # Validate input directory
    input_path = Path(args.directory).resolve()
    if not input_path.exists():
        print(f"❌ Error: Directory '{args.directory}' does not exist!")
        return 1

    if not input_path.is_dir():
        print(f"❌ Error: '{args.directory}' is not a directory!")
        return 1

    # Determine comment removal setting
    remove_comments = None
    if args.remove_comments:
        remove_comments = True
    elif args.keep_comments:
        remove_comments = False

    # Determine JS inclusion
    include_js: Optional[bool] = None
    if args.exclude_js:
        include_js = False
    elif args.include_js:
        include_js = True

    # Decide whether to present a full interactive menu:
    # If the user provided none of the option flags, we show the full menu.
    no_cli_options = (
        (remove_comments is None) and
        (args.md == "readme") and
        (include_js is None) and
        (args.output == "code_structure_analysis.md")
    )

    analyzer = CodeAnalyzer(
        str(input_path),
        args.output,
        remove_comments if remove_comments is not None else False,
        md_mode=args.md,
        include_js=True if include_js is None else include_js,
        interactive_menu=no_cli_options
    )

    # Avoid re-asking if specified in command line
    if remove_comments is not None:
        analyzer.remove_comments = remove_comments
        analyzer._comment_choice_made = True

    try:
        success = analyzer.generate_output_file()
        return 0 if success else 1

    except KeyboardInterrupt:
        print("\n⚠️ Analysis interrupted by user")
        return 1
    except Exception as e:
        print(f"❌ Unexpected error: {str(e)}")
        return 1

if __name__ == "__main__":
    raise SystemExit(main())
