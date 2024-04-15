using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using iText;
using iText.IO.Image;
using iText.IO.Source;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Utils;
using iText.Layout;
using iText.Layout.Element;

namespace App.PdfUtils
{
    /// <summary>
    /// Classe per eseguire la conversione di tipi doc -> PDF
    /// </summary>
    public static class PdfTool
    {

        /// <summary>
        /// Crea un pdf dalle pagine specificate
        /// </summary>
        /// <param name="pdfStream"></param>
        /// <param name="pages"></param>
        /// <returns></returns>
        public static MemoryStream NewPdfFromPages(Stream pdfStream, IEnumerable<int> pages)
        {
            using (var rd = new PdfReader(pdfStream))
            {
                rd.SetUnethicalReading(true);
                using (var doc = new PdfDocument(rd))
                {
                    var iinfo = doc.GetDocumentInfo();
                    using (var ms = new MemoryStream())
                    {
                        using (var w = new PdfWriter(ms))
                        {
                            w.SetCloseStream(false);
                            using (var pdf = new PdfDocument(w))
                            {
                                var oinfo = doc.GetDocumentInfo();
                                oinfo.SetAuthor(iinfo.GetAuthor());
                                oinfo.SetTitle(iinfo.GetTitle());
                                oinfo.SetSubject(iinfo.GetSubject());
                                // copy template pages 1..1 to pdf as target page 1 onwards
                                foreach (var page in pages)
                                {
                                    doc.CopyPagesTo(page, page, pdf);
                                }
                            }
                        }

                        //Riposiziona all'inizio e esce
                        ms.Position = 0;
                        return ms;
                    }


                }
            }
        }

        /// <summary>
        /// Crea un pdf dalle pagine specificate
        /// </summary>
        /// <param name="pdfBuffer"></param>
        /// <param name="pages"></param>
        /// <returns></returns>
        public static byte[] NewPdfFromPages(byte[] pdfBuffer, IEnumerable<int> pages)
        {
            using (var ms = new MemoryStream(pdfBuffer))
            {
                using (var msOut = NewPdfFromPages(ms, pages))
                {
                    return msOut.ToArray();
                } 
            }
        }



        #region Numero Pagine

        /// <summary>
        /// Ritorna il numero di pagine
        /// </summary>
        /// <param name="pdfStream"></param>
        /// <returns></returns>
        public static int GetNumberOfPages(Stream pdfStream)
        {

            using (var rd = new PdfReader(pdfStream))
            {
                rd.SetUnethicalReading(true);
                using (var doc = new PdfDocument(rd))
                {
                    return doc.GetNumberOfPages();
                }
            }
        }

        /// <summary>
        /// Ritorna il numero di pagine
        /// </summary>
        /// <param name="pdfStream"></param>
        /// <returns></returns>
        public static int GetNumberOfPages(string pdfPath)
        {
            using (var fs = File.Open(pdfPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return GetNumberOfPages(fs);
            }
        }

        /// <summary>
        /// Ritorna il numero di pagine
        /// </summary>
        /// <param name="pdfStream"></param>
        /// <returns></returns>
        public static int GetNumberOfPages(byte[] pdfBuffer)
        {
            using (var ms = new MemoryStream(pdfBuffer))
            {
                return GetNumberOfPages(ms);
            }
        }

        #endregion


        #region Text Extract

        /// <summary>
        /// Estrae testo da PDF
        /// </summary>
        /// <param name="pdfStream"></param>
        /// <returns></returns>
        public static PdfDocText PdfExtractText(Stream pdfStream)
        {
            var ret = new PdfDocText();

            using (var rd = new PdfReader(pdfStream))
            {
                rd.SetUnethicalReading(true);
                using (var doc = new PdfDocument(rd))
                {
                    for (int i = 1; i <= doc.GetNumberOfPages(); i++)
                    {
                        ret.Pages.Add(new PdfPageText
                        {
                            PageNum = i,
                            Text = PdfTextExtractor.GetTextFromPage(doc.GetPage(i), new SimpleTextExtractionStrategy())
                        });
                    }
                }
            }

            return ret;
        }


        /// <summary>
        /// Estrae testo da PDF
        /// </summary>
        /// <param name="pdfStream"></param>
        /// <returns></returns>
        public static PdfDocText PdfExtractText(byte[] pdfBuffer)
        {
            using (var ms = new MemoryStream(pdfBuffer))
            {
                return PdfExtractText(ms);
            }
        }

        #endregion


    }
}
