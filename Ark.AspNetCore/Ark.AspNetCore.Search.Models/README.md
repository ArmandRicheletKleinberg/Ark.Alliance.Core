# Ark.AspNetCore.Search.Models

A minimal package that ships the JSON dataset used by `SearchItemTypeEnum`.
It provides the default set of search item types loaded by the `Ark.AspNetCore.Search`
library. The data is embedded as `Resources/SearchItemTypes.json` and can be
loaded at runtime to populate `SearchItemTypeEnum.All`.

No code is included apart from the resource file and project metadata. This
package simply centralizes the default values so they can be shared between
applications and tests.
