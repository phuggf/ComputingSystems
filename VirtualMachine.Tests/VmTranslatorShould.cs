using System.Diagnostics;
using System.Text;

namespace VirtualMachine.Tests
{
    [TestClass]
    public class VmTranslatorShould
    {
        [TestMethod]
        public void TranslateIntermediateLanguageToAssembly()
        {
            string directoryPath = @"C:\Users\Ulric\OneDrive\Documents\Projects\nand2tetris\nand2tetris\projects\07\StackArithmetic\StackTest";
            string testName = "StackTest";
            string[] inputFileNames = { "StackTest.vm" };
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
                // Prepend line numbers to each line of the output.
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