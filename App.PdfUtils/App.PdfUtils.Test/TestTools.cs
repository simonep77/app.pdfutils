using Microsoft.VisualStudio.TestTools.UnitTesting;
using App.PdfUtils;
using System;
using System.IO;

namespace App.PdfUtils.Test
{
    [TestClass]
    public class TestTools
    {
        [TestMethod]
        public void Test1()
        {

            var i = PdfTool.GetNumberOfPages(TestResources.TestPng);
            var blob = PdfTool.NewPdfFromPages(TestResources.TestPng, new int[] { 1 });
            var i2 = PdfTool.GetNumberOfPages(blob);

        }


    }
}
