using System.Diagnostics;
using System.Text;

namespace VirtualMachine.Tests
{
    [TestClass]
    public class VmTranslatorShould
    {
        /// <summary>
        /// Integration test that writes a file of the VmTranslator output and runs it through
        /// the CPU emulator supplied with the course materials. Only un-ignore if the correct
        /// .vm files are placed in the test bin folder.
        /// </summary>
        [TestMethod]
        [Ignore]
        public void TranslateIntermediateLanguageToAssembly()
        {
            string directoryPath = @"C:\Users\Ulric\OneDrive\Documents\Projects\nand2tetris\nand2tetris\projects\08\FunctionCalls\FibonacciElement";
            string testName = "FibonacciElement";
            string[] inputFileNames = { "Sys.vm", "Main.vm" };
            string outputFileName = $"{testName}.asm";

            var vmTranslator = new VmTranslator();
            List<VmFile> vmFiles = [];

            foreach (var fileName in inputFileNames)
            {
                var contents = File.ReadAllLines(fileName);
                vmFiles.Add(new VmFile(fileName, contents));
            }

            IEnumerable<string> assembly = vmTranslator.ConvertToAssembly(vmFiles);
            
            string fullOutputPath = Path.Combine(directoryPath, outputFileName);
            string fullTestFilePath = Path.Combine(directoryPath, $"{testName}.tst");
            File.WriteAllLines(fullOutputPath, assembly);
            var error = RunTestFile(fullTestFilePath);
            Assert.AreEqual(string.Empty, error);
        }

        /// <summary>
        /// Run the CPUEmulator supplied with the course through cmd and give it the test script.
        /// </summary>
        private static string RunTestFile(string fullTestFilePath)
        {
            ProcessStartInfo cmdStartInfo = new()
            {
                FileName = @"C:\Windows\System32\cmd.exe",
                RedirectStandardError = true,
                RedirectStandardInput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            Process process = new()
            {
                StartInfo = cmdStartInfo
            };

            var stdErr = new StringBuilder();
            process.ErrorDataReceived += new DataReceivedEventHandler((sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    stdErr.AppendLine(e.Data);
                }
            });

            process.Start();
            process.BeginErrorReadLine();
            process.StandardInput.WriteLine(@"C:\Users\Ulric\OneDrive\Documents\Projects\nand2tetris\nand2tetris\tools\CPUEmulator" + $" {fullTestFilePath}");
            process.WaitForExit(1000);

            process.Close();
            return stdErr.ToString();
        }
    }
}