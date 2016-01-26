﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using BenchmarkDotNet.Helpers;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Reports;

namespace BenchmarkDotNet.Exporters
{
    public class RPlotExporter : IExporter
    {
        public static readonly IExporter Default = new RPlotExporter();

        public IEnumerable<string> ExportToFiles(Summary summary)
        {
            const string scriptFileName = "BuildPlots.R";
            yield return scriptFileName;

            var fileNamePrefix = Path.Combine(summary.CurrentDirectory, summary.Title);
            var scriptFullPath = Path.Combine(summary.CurrentDirectory, scriptFileName);
            File.WriteAllText(scriptFullPath, ResourceHelper.LoadTemplate(scriptFileName));

            var rHome = Environment.GetEnvironmentVariable("R_HOME") ?? @"C:\Program Files\R\R-3.2.3\bin\";
            if (Directory.Exists(rHome))
            {
                var start = new ProcessStartInfo
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = false,
                    CreateNoWindow = true,
                    FileName = Path.Combine(rHome, "Rscript.exe"),
                    Arguments = $"{scriptFullPath} {fileNamePrefix}-runs.csv"
                };
                using (var process = Process.Start(start))
                {
                }
                yield return fileNamePrefix + "-boxplot.png";
                yield return fileNamePrefix + "-barplot.png";
            }
        }

        public void ExportToLog(Summary summary, ILogger logger)
        {
            throw new NotSupportedException();
        }
    }
}