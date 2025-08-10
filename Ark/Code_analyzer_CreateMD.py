#!/usr/bin/env python3
"""
Code Structure and Content Analyzer
Generates a comprehensive MD file with directory structure and all code file contents
Supports comment removal for clean documentation
"""

import os
import re
import argparse
from pathlib import Path
from datetime import datetime
from typing import List, Dict, Tuple, Set

class CodeAnalyzer:
    def __init__(self, root_path: str, output_file: str = "code_structure_analysis.md", remove_comments: bool = False):
        self.root_path = Path(root_path).resolve()
        self.output_file = Path(output_file).resolve()
        self.remove_comments = remove_comments
        self.code_files = []
        
        # Supported code file extensions
        self.code_extensions = {
            '.sln', '.csproj', '.cs', '.js', '.py', '.razor', '.jsx', '.tsx', '.ts', '.json'
        }
        
        # File type mappings for syntax highlighting
        self.syntax_mapping = {
            '.sln': 'text',
            '.csproj': 'xml',
            '.cs': 'csharp',
            '.js': 'javascript',
            '.jsx': 'jsx',
            '.tsx': 'tsx',
            '.ts': 'typescript',
            '.py': 'python',
            '.razor': 'html',
            '.json': 'json'
        }
        
    def is_code_file(self, file_path: Path) -> bool:
        """Check if file is a code file we want to analyze"""
        return file_path.suffix.lower() in self.code_extensions
    
    def should_exclude_file(self, file_path: Path) -> bool:
        """Check if file should be excluded (build artifacts, etc.)"""
        exclude_patterns = {
            # Build/Output directories
            'bin', 'obj', 'dist', 'build', 'out', 'target',
            # Dependencies
            'node_modules', 'packages', '__pycache__', '.pytest_cache',
            # IDE/Editor files
            '.vs', '.vscode', '.idea',
            # Version control
            '.git', '.svn', '.hg',
            # Other
            '.azure', '.env', 'logs'
        }
        
        # Check if any part of the path contains excluded patterns
        path_parts = set(file_path.parts)
        if path_parts.intersection(exclude_patterns):
            return True
            
        # Exclude specific file patterns
        exclude_file_patterns = {
            '.min.js', '.min.css', 'bundle.js', 'bundle.css',
            'package-lock.json', 'yarn.lock', 'pnpm-lock.yaml'
        }
        
        for pattern in exclude_file_patterns:
            if pattern in file_path.name.lower():
                return True
                
        return False
    
    def remove_comments_from_content(self, content: str, file_extension: str) -> str:
        """Remove comments from code content based on file type"""
        if not self.remove_comments:
            return content
            
        if file_extension in ['.cs', '.razor']:
            return self._remove_csharp_comments(content)
        elif file_extension in ['.js', '.jsx', '.ts', '.tsx']:
            return self._remove_javascript_comments(content)
        elif file_extension == '.py':
            return self._remove_python_comments(content)
        elif file_extension in ['.json']:
            # JSON doesn't officially support comments, but some tools allow them
            return self._remove_json_comments(content)
        elif file_extension in ['.sln', '.csproj']:
            # MSBuild/Solution files - be careful with XML comments
            return self._remove_xml_comments(content)
        else:
            return content
    
    def _remove_csharp_comments(self, content: str) -> str:
        """Remove C# comments including XML documentation"""
        lines = content.split('\n')
        result_lines = []
        in_multiline_comment = False
        
        for line in lines:
            original_line = line
            line = line.rstrip()
            
            if in_multiline_comment:
                # Look for end of multiline comment
                end_pos = line.find('*/')
                if end_pos != -1:
                    line = line[end_pos + 2:]
                    in_multiline_comment = False
                else:
                    continue
            
            # Remove XML documentation comments (///)
            if line.strip().startswith('///'):
                continue
                
            # Remove single line comments (//)
            # Be careful not to remove // in strings
            in_string = False
            escaped = False
            comment_pos = -1
            
            for i, char in enumerate(line):
                if escaped:
                    escaped = False
                    continue
                    
                if char == '\\':
                    escaped = True
                    continue
                    
                if char == '"' and not escaped:
                    in_string = not in_string
                    continue
                    
                if not in_string and i < len(line) - 1:
                    if line[i:i+2] == '//':
                        comment_pos = i
                        break
                    elif line[i:i+2] == '/*':
                        # Start of multiline comment
                        end_pos = line.find('*/', i + 2)
                        if end_pos != -1:
                            # Comment ends on same line
                            line = line[:i] + line[end_pos + 2:]
                            break
                        else:
                            # Comment continues to next line
                            line = line[:i]
                            in_multiline_comment = True
                            break
            
            if comment_pos != -1:
                line = line[:comment_pos].rstrip()
                
            # Only add non-empty lines or preserve original spacing for structure
            if line.strip() or not original_line.strip():
                result_lines.append(line)
        
        return '\n'.join(result_lines)
    
    def _remove_javascript_comments(self, content: str) -> str:
        """Remove JavaScript/TypeScript comments"""
        lines = content.split('\n')
        result_lines = []
        in_multiline_comment = False
        
        for line in lines:
            original_line = line
            line = line.rstrip()
            
            if in_multiline_comment:
                end_pos = line.find('*/')
                if end_pos != -1:
                    line = line[end_pos + 2:]
                    in_multiline_comment = False
                else:
                    continue
            
            # Handle comments while preserving strings
            in_string = False
            in_regex = False
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
                    
                if char == '\\':
                    escaped = True
                    i += 1
                    continue
                
                if not in_string and not in_regex:
                    if char in ['"', "'", '`']:
                        in_string = True
                        string_char = char
                    elif char == '/' and i < len(line) - 1:
                        next_char = line[i + 1]
                        if next_char == '/':
                            comment_pos = i
                            break
                        elif next_char == '*':
                            end_pos = line.find('*/', i + 2)
                            if end_pos != -1:
                                line = line[:i] + line[end_pos + 2:]
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
        
        return '\n'.join(result_lines)
    
    def _remove_python_comments(self, content: str) -> str:
        """Remove Python comments"""
        lines = content.split('\n')
        result_lines = []
        in_multiline_string = False
        multiline_char = None
        
        for line_num, line in enumerate(lines):
            original_line = line
            line = line.rstrip()
            
            # Handle multiline strings (which might contain # that aren't comments)
            if in_multiline_string:
                if multiline_char * 3 in line:
                    in_multiline_string = False
                    multiline_char = None
                result_lines.append(original_line.rstrip())
                continue
            
            # Check for start of multiline string
            for quote in ['"""', "'''"]:
                if quote in line:
                    # Simple check - in real code this would be more complex
                    if line.count(quote) % 2 == 1:  # Odd number means string starts
                        in_multiline_string = True
                        multiline_char = quote[0]
                        result_lines.append(original_line.rstrip())
                        break
            else:
                # Remove single line comments (#)
                in_string = False
                escaped = False
                string_char = None
                comment_pos = -1
                
                for i, char in enumerate(line):
                    if escaped:
                        escaped = False
                        continue
                        
                    if char == '\\':
                        escaped = True
                        continue
                    
                    if not in_string:
                        if char in ['"', "'"]:
                            in_string = True
                            string_char = char
                        elif char == '#':
                            comment_pos = i
                            break
                    elif char == string_char:
                        in_string = False
                        string_char = None
                
                if comment_pos != -1:
                    line = line[:comment_pos].rstrip()
                    
                if line.strip() or not original_line.strip():
                    result_lines.append(line)
        
        return '\n'.join(result_lines)
    
    def _remove_json_comments(self, content: str) -> str:
        """Remove comments from JSON-like files (non-standard but sometimes used)"""
        lines = content.split('\n')
        result_lines = []
        
        for line in lines:
            original_line = line
            # Remove // comments in JSON (non-standard)
            comment_pos = line.find('//')
            if comment_pos != -1:
                # Make sure it's not in a string
                quote_count = line[:comment_pos].count('"')
                if quote_count % 2 == 0:  # Even number of quotes means we're not in a string
                    line = line[:comment_pos].rstrip()
            
            if line.strip() or not original_line.strip():
                result_lines.append(line)
        
        return '\n'.join(result_lines)
    
    def _remove_xml_comments(self, content: str) -> str:
        """Remove XML comments from .csproj, .sln files"""
        # Remove XML comments <!-- -->
        content = re.sub(r'<!--.*?-->', '', content, flags=re.DOTALL)
        
        # Clean up empty lines
        lines = content.split('\n')
        result_lines = []
        for line in lines:
            if line.strip() or not line:
                result_lines.append(line.rstrip())
        
        return '\n'.join(result_lines)
    
    def generate_directory_tree(self) -> List[str]:
        """Generate a visual directory tree structure for code files only"""
        tree_lines = []
        
        def add_directory_tree(directory: Path, prefix: str = "", is_last: bool = True):
            if directory.name.startswith('.') or self.should_exclude_file(directory):
                return
                
            # Check if directory contains any code files
            has_code_files = any(
                self.is_code_file(f) and not self.should_exclude_file(f)
                for f in directory.rglob('*') if f.is_file()
            )
            
            if not has_code_files:
                return
                
            connector = "└── " if is_last else "├── "
            tree_lines.append(f"{prefix}{connector}{directory.name}/")
            
            try:
                items = list(directory.iterdir())
                items = [item for item in items if not item.name.startswith('.') and not self.should_exclude_file(item)]
                items.sort(key=lambda x: (x.is_file(), x.name.lower()))
                
                directories = [item for item in items if item.is_dir()]
                files = [item for item in items if item.is_file() and self.is_code_file(item)]
                
                # Filter directories that contain code files
                code_directories = []
                for subdir in directories:
                    if any(self.is_code_file(f) and not self.should_exclude_file(f) 
                           for f in subdir.rglob('*') if f.is_file()):
                        code_directories.append(subdir)
                
                # Add directories
                for i, subdir in enumerate(code_directories):
                    is_last_dir = (i == len(code_directories) - 1) and len(files) == 0
                    new_prefix = prefix + ("    " if is_last else "│   ")
                    add_directory_tree(subdir, new_prefix, is_last_dir)
                
                # Add code files
                for i, file in enumerate(files):
                    is_last_file = i == len(files) - 1
                    file_connector = "└── " if is_last_file else "├── "
                    new_prefix = prefix + ("    " if is_last else "│   ")
                    
                    try:
                        size = file.stat().st_size
                        size_str = self.format_file_size(size)
                        tree_lines.append(f"{new_prefix}{file_connector}{file.name} ({size_str})")
                    except:
                        tree_lines.append(f"{new_prefix}{file_connector}{file.name}")
                        
            except PermissionError:
                tree_lines.append(f"{prefix}    [Permission Denied]")
        
        # Start with root
        tree_lines.append(f"{self.root_path.name}/")
        
        try:
            items = list(self.root_path.iterdir())
            items = [item for item in items if not item.name.startswith('.') and not self.should_exclude_file(item)]
            items.sort(key=lambda x: (x.is_file(), x.name.lower()))
            
            directories = [item for item in items if item.is_dir()]
            files = [item for item in items if item.is_file() and self.is_code_file(item)]
            
            # Filter directories
            code_directories = []
            for subdir in directories:
                if any(self.is_code_file(f) and not self.should_exclude_file(f) 
                       for f in subdir.rglob('*') if f.is_file()):
                    code_directories.append(subdir)
            
            # Process directories
            for i, subdir in enumerate(code_directories):
                is_last_dir = (i == len(code_directories) - 1) and len(files) == 0
                add_directory_tree(subdir, "", is_last_dir)
            
            # Process files
            for i, file in enumerate(files):
                is_last_file = i == len(files) - 1
                connector = "└── " if is_last_file else "├── "
                try:
                    size = file.stat().st_size
                    size_str = self.format_file_size(size)
                    tree_lines.append(f"{connector}{file.name} ({size_str})")
                except:
                    tree_lines.append(f"{connector}{file.name}")
                    
        except PermissionError:
            tree_lines.append("[Permission Denied]")
            
        return tree_lines
    
    def format_file_size(self, size_bytes: int) -> str:
        """Format file size in human-readable format"""
        if size_bytes == 0:
            return "0 B"
        
        size_names = ["B", "KB", "MB", "GB"]
        i = 0
        while size_bytes >= 1024 and i < len(size_names) - 1:
            size_bytes /= 1024.0
            i += 1
        
        if i == 0:
            return f"{int(size_bytes)} {size_names[i]}"
        else:
            return f"{size_bytes:.1f} {size_names[i]}"
    
    def find_code_files(self) -> Dict[str, List[Path]]:
        """Find all code files organized by directory"""
        code_files_by_dir = {}
        
        for root, dirs, files in os.walk(self.root_path):
            # Skip excluded directories
            dirs[:] = [d for d in dirs if not d.startswith('.') and not self.should_exclude_file(Path(root) / d)]
            
            root_path = Path(root)
            if self.should_exclude_file(root_path):
                continue
                
            relative_path = root_path.relative_to(self.root_path)
            
            code_files_in_dir = []
            for file in files:
                if not file.startswith('.'):
                    file_path = root_path / file
                    if self.is_code_file(file_path) and not self.should_exclude_file(file_path):
                        code_files_in_dir.append(file_path)
            
            if code_files_in_dir:
                code_files_in_dir.sort(key=lambda x: x.name.lower())
                code_files_by_dir[str(relative_path)] = code_files_in_dir
        
        return code_files_by_dir
    
    def read_code_file(self, file_path: Path) -> str:
        """Read code file content safely"""
        try:
            with open(file_path, 'r', encoding='utf-8') as f:
                content = f.read()
        except UnicodeDecodeError:
            try:
                with open(file_path, 'r', encoding='latin-1') as f:
                    content = f.read()
            except Exception as e:
                return f"Error reading file: {str(e)}"
        except Exception as e:
            return f"Error reading file: {str(e)}"
        
        # Remove comments if requested
        return self.remove_comments_from_content(content, file_path.suffix.lower())
    
    def generate_statistics(self, code_files_by_dir: Dict[str, List[Path]]) -> Dict:
        """Generate project statistics"""
        stats = {
            'total_directories': len(code_files_by_dir),
            'total_code_files': 0,
            'total_size': 0,
            'file_types': {}
        }
        
        for dir_files in code_files_by_dir.values():
            stats['total_code_files'] += len(dir_files)
            
            for file_path in dir_files:
                try:
                    stats['total_size'] += file_path.stat().st_size
                    ext = file_path.suffix.lower()
                    stats['file_types'][ext] = stats['file_types'].get(ext, 0) + 1
                except:
                    pass
        
        return stats
    
    def get_syntax_highlight(self, file_path: Path) -> str:
        """Get syntax highlighting language for file"""
        return self.syntax_mapping.get(file_path.suffix.lower(), 'text')
    
    def generate_output_file(self):
        """Generate the comprehensive output markdown file"""
        print(f"🔍 Analyzing code structure in: {self.root_path}")
        
        # Ask user about comment removal if not specified
        if not hasattr(self, '_comment_choice_made'):
            while True:
                choice = input("\n❓ Remove comments and documentation from code? (y/n): ").strip().lower()
                if choice in ['y', 'yes']:
                    self.remove_comments = True
                    break
                elif choice in ['n', 'no']:
                    self.remove_comments = False
                    break
                else:
                    print("Please enter 'y' for yes or 'n' for no.")
        
        # Generate directory tree
        print("📂 Generating directory tree for code files...")
        tree_structure = self.generate_directory_tree()
        
        # Find all code files
        print("💻 Finding code files...")
        code_files_by_dir = self.find_code_files()
        
        # Generate statistics
        print("📊 Calculating statistics...")
        stats = self.generate_statistics(code_files_by_dir)
        
        # Create output content
        print("✍️ Generating output file...")
        
        output_lines = []
        
        # Header
        comment_status = "Comments Removed" if self.remove_comments else "Comments Preserved"
        output_lines.extend([
            f"# Code Structure Analysis",
            f"",
            f"**Generated on:** {datetime.now().strftime('%Y-%m-%d %H:%M:%S')}",
            f"**Root Directory:** `{self.root_path}`",
            f"**Comment Processing:** {comment_status}",
            f"**Analysis Tool:** Code Structure and Content Analyzer",
            f"",
            "---",
            ""
        ])
        
        # Statistics section
        output_lines.extend([
            "## 📊 Project Statistics",
            "",
            f"- **Total Directories with Code:** {stats['total_directories']}",
            f"- **Total Code Files:** {stats['total_code_files']}",
            f"- **Total Code Size:** {self.format_file_size(stats['total_size'])}",
            ""
        ])
        
        # File type breakdown
        if stats['file_types']:
            output_lines.extend([
                "### File Type Breakdown",
                ""
            ])
            for ext, count in sorted(stats['file_types'].items()):
                output_lines.append(f"- **{ext}**: {count} files")
            output_lines.extend(["", "---", ""])
        
        # Directory structure section
        output_lines.extend([
            "## 📁 Code Directory Structure",
            "",
            "Complete recursive directory structure (code files only):",
            "",
            "```",
            *tree_structure,
            "```",
            "",
            "---",
            ""
        ])
        
        # Code files content section
        output_lines.extend([
            "## 💻 Code Files Content",
            "",
            f"Complete content of all code files {'(comments removed)' if self.remove_comments else '(with comments)'}:",
            ""
        ])
        
        # Process each directory with code files
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
            
            output_lines.extend([
                f"### {display_path}",
                "",
                f"**Full Path:** `{full_path}`",
                f"**Code Files:** {len(files)}",
                ""
            ])
            
            # Process each code file in this directory
            for j, file_path in enumerate(files):
                relative_file_path = file_path.relative_to(self.root_path)
                syntax = self.get_syntax_highlight(file_path)
                
                output_lines.extend([
                    f"#### 💾 {file_path.name}",
                    "",
                    f"**File Path:** `{relative_file_path}`",
                    f"**File Type:** {file_path.suffix.upper()} ({syntax})",
                    ""
                ])
                
                # Read and include file content
                content = self.read_code_file(file_path)
                
                output_lines.extend([
                    f"```{syntax}",
                    content,
                    "```",
                    ""
                ])
                
                # Add separator between files
                if not (i == len(sorted_dirs) - 1 and j == len(files) - 1):
                    output_lines.extend(["---", ""])
        
        # Footer
        output_lines.extend([
            "",
            "---",
            "",
            f"**Analysis completed on {datetime.now().strftime('%Y-%m-%d at %H:%M:%S')}**",
            f"**Comments:** {'Removed for clean documentation' if self.remove_comments else 'Preserved as written'}",
            "",
            f"*Generated by Code Structure and Content Analyzer*"
        ])
        
        # Write to output file
        print(f"💾 Writing to: {self.output_file}")
        try:
            with open(self.output_file, 'w', encoding='utf-8') as f:
                f.write('\n'.join(output_lines))
            
            print(f"✅ Analysis complete!")
            print(f"📄 Output file: {self.output_file}")
            print(f"💻 Processed {stats['total_code_files']} code files from {len(code_files_by_dir)} directories")
            print(f"🗂️ File types: {', '.join(stats['file_types'].keys())}")
            
        except Exception as e:
            print(f"❌ Error writing output file: {str(e)}")
            return False
        
        return True

def main():
    """Main function with command line argument parsing"""
    parser = argparse.ArgumentParser(
        description="Analyze project code structure and consolidate all code files",
        formatter_class=argparse.RawDescriptionHelpFormatter,
        epilog="""
Supported file types:
  .sln, .csproj, .cs, .js, .jsx, .ts, .tsx, .py, .razor, .json

Examples:
  python code_analyzer.py                              # Analyze current directory
  python code_analyzer.py /path/to/project             # Analyze specific directory
  python code_analyzer.py . -o code_analysis.md        # Custom output file
  python code_analyzer.py . --remove-comments          # Remove all comments
  python code_analyzer.py . --keep-comments            # Keep all comments
        """
    )
    
    parser.add_argument(
        'directory',
        nargs='?',
        default='.',
        help='Directory to analyze (default: current directory)'
    )
    
    parser.add_argument(
        '-o', '--output',
        default='code_structure_analysis.md',
        help='Output file name (default: code_structure_analysis.md)'
    )
    
    comment_group = parser.add_mutually_exclusive_group()
    comment_group.add_argument(
        '--remove-comments',
        action='store_true',
        help='Remove all comments and documentation from code'
    )
    comment_group.add_argument(
        '--keep-comments',
        action='store_true',
        help='Keep all comments and documentation in code'
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
    
    # Create analyzer and run analysis
    analyzer = CodeAnalyzer(str(input_path), args.output, remove_comments if remove_comments is not None else False)
    
    # Set flag to avoid re-asking if specified in command line
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
    exit(main())
