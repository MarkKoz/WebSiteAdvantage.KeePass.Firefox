# WebSiteAdvantage.KeePass.Firefox
Core Library for the Firefox to KeePass Password Importer.

## Notes
`System.Data.SQLite.Core` relies on `SQLite.Interop.dll`. However, it will not
automatically copy it to the build's output directory for projects which
reference this project. Therefore, it is one's responsibility to ensure
`SQLite.Interop.dll` ends up in the build directory of one's project. A simple
solution is to add the `System.Data.SQLite.Core` package as a dependency to
one's project, even if it doesn't use SQLite itself.
