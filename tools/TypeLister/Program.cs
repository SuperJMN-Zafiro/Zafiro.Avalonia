using System.Reflection;

if (args.Length == 0)
{
    Console.WriteLine("Usage: TypeLister <path-to-assembly>");
    return 1;
}

var path = args[0];
if (!File.Exists(path))
{
    Console.WriteLine($"File not found: {path}");
    return 2;
}

// Setup simple resolver: try same directory first, then NuGet cache siblings
AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
{
    try
    {
        var requestedName = new AssemblyName(args.Name).Name + ".dll";
        var dir = Path.GetDirectoryName(path)!;
        var candidate = Path.Combine(dir, requestedName);
        if (File.Exists(candidate)) return Assembly.LoadFrom(candidate);

        // Try locate under NuGet packages root
        // Go up to packages root from the given path: .../packages/<id>/<version>/lib/net8.0/file.dll
        var nugetLibDir = Directory.GetParent(dir) // net8.0
                             ?.Parent            // lib
                             ?.Parent            // version
                             ?.Parent            // package id
                             ?.Parent            // packages root
                             ?.FullName;
        if (nugetLibDir is not null)
        {
            var pkgId = requestedName.Replace(".dll", "", StringComparison.Ordinal);
            // Map assembly name to package id casing
            var packageGuess = pkgId.ToLowerInvariant();
            var searchRoot = Path.Combine(nugetLibDir, packageGuess);
            if (!Directory.Exists(searchRoot))
            {
                // Fallback: search entire packages root (limited depth) for requestedName
                var match = Directory.EnumerateFiles(nugetLibDir, requestedName, SearchOption.AllDirectories)
                    .FirstOrDefault(p => p.Contains(Path.Combine("lib", "net8.0"), StringComparison.OrdinalIgnoreCase));
                if (match is not null) return Assembly.LoadFrom(match);
            }
            else
            {
                var match = Directory.EnumerateFiles(searchRoot, requestedName, SearchOption.AllDirectories)
                    .FirstOrDefault(p => p.Contains(Path.Combine("lib", "net8.0"), StringComparison.OrdinalIgnoreCase));
                if (match is not null) return Assembly.LoadFrom(match);
            }
        }
    }
    catch { }
    return null;
};

try
{
    var asm = Assembly.LoadFrom(path);
    Console.WriteLine($"Loaded: {asm.FullName}");
    var types = asm.GetExportedTypes().OrderBy(t => t.FullName).ToList();
    foreach (var t in types)
    {
        Console.WriteLine(t.FullName);
    }
}
catch (Exception ex)
{
    Console.WriteLine($"ERROR: {ex}");
    return 3;
}

return 0;
