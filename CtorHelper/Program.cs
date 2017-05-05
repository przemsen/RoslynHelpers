using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using System;
using System.Linq;
using static System.Console;

namespace CtorHelper
{
    class Program
    {
        static void Main(string[] args)
        {
            string pathToSolution, projectName, classFullName;

#if !DEBUG
            if (args.Count() != 3)
            {
                WriteLine("Generates constructor call for given type with typing comments");
                WriteLine("Usage: CtorHelper (solution path) (project name) (class name including namespace)");
                return;
            }

            pathToSolution = args[0];
            projectName = args[1];
            classFullName = args[2];
#else

            pathToSolution = @"";
            projectName = "";
            classFullName = @"";
#endif

            MSBuildWorkspace workspace = MSBuildWorkspace.Create();
            Solution solutionToAnalyze = workspace.OpenSolutionAsync(pathToSolution).Result;
            Project sampleProjectToAnalyze = solutionToAnalyze.Projects.Single((proj) => proj.Name == projectName);
            Compilation compilation = sampleProjectToAnalyze.GetCompilationAsync().Result;

            var t = compilation.GetTypeByMetadataName(classFullName);
            var c = t.Constructors.First();

            WriteLine("new " + t.Name + "(");

            foreach(var p in c.Parameters)
            {
                var pt = p.Type.ToDisplayString();
                var gp = pt.IndexOf('<', 0);

                if (gp > 0)
                {
                    var gep = pt.IndexOf('>', 0);
                    var genericTypeName = pt.Substring(gp + 1, gep - gp - 1);
                    WriteLine($"  null, // {p.Type.Name}<{genericTypeName.Split('.').Last()}>");
                }
                else
                {
                    WriteLine($"  null, // {p.Type.Name}");
                }
            }

            WriteLine(");");

#if DEBUG
            ReadKey();
#endif
        }
    }
}
