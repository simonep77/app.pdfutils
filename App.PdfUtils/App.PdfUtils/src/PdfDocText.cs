using System;
using System.Collections.Generic;
using System.Text;

namespace App.PdfUtils
{
    /// <summary>
    /// CLasse che contiene le pagine con il testo estratto dal pdf
    /// </summary>
    public class PdfDocText
    {
        public List<PdfPageText> Pages { get; set; } = new List<PdfPageText>();
    }

    /// <summary>
    /// Estrazione di testo da singola pagina
    /// </summary>
    public class PdfPageText
    {
        public int PageNum { get; set; }
        public string Text { get; set; }
    }

}
