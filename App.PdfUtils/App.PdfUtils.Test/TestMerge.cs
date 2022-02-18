using Microsoft.VisualStudio.TestTools.UnitTesting;
using App.PdfUtils;
using System;
using System.IO;

namespace App.PdfUtils.Test
{
    [TestClass]
    public class TestMerge
    {
        [TestMethod]
        public void Test1()
        {

            var pdfmerge = new PdfMerge();
            pdfmerge.BeginMergeToFile(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "TestMerge.pdf"));
            pdfmerge.SetPdfInfo("Merge 1", "Simone pelaia", "Merge");

            pdfmerge.AddPdfFromBuffer(TestResources.TestPng);
            pdfmerge.AddPdfFromBuffer(TestResources.TestPng);
            pdfmerge.AddPdfFromBuffer(TestResources.TestPng);
            pdfmerge.AddPdfFromBuffer(TestResources.TestPng);
            pdfmerge.AddBlankPageWithText("Ciao Simone!!!! Ciao Simone!!!! Ciao Simone!!!! Ciao Simone!!!! Ciao Simone!!!! Ciao Simone!!!! Ciao Simone!!!! Ciao Simone!!!! Ciao Simone!!!! Ciao Simone!!!! Ciao Simone!!!! \nCiao Simone!!!! Ciao Simone!!!! Ciao Simone!!!! Ciao Simone!!!! Ciao Simone!!!! Ciao Simone!!!! Ciao Simone!!!! Ciao Simone!!!! Ciao Simone!!!! Ciao Simone!!!! ");

            pdfmerge.AddPdfFromImageBuffer(TestResources.Prova);

            pdfmerge.EndMerge();
        }
    }
}
