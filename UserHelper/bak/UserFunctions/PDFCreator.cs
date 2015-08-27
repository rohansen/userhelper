using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserHelper.Models;

namespace UserHelper.UserFunctions
{
    public class PDFCreator
    {

        public static void CreatePDF(Student[] students)
        {
            try { 
            FileStream fs = new FileStream("StudentList.pdf", FileMode.Create, FileAccess.Write, FileShare.None);
            Document doc = new Document();
            doc.SetPageSize(iTextSharp.text.PageSize.A4.Rotate());
            PdfWriter writer = PdfWriter.GetInstance(doc, fs);

            PdfPTable table = new PdfPTable(7);
            table.TotalWidth = 800f;
            table.DefaultCell.FixedHeight = 110f;
            table.LockedWidth = true;
            float[] widths = { 2f, 2f,2f,2f,2f,2f,2f};
            table.SetWidths(widths);
            table.HorizontalAlignment = 0;

            //PdfPCell cell = new PdfPCell();
            //cell.Colspan = 3;
            //cell.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
            Paragraph para = new Paragraph("Holdliste for: " + students[0].Team.Name);


            foreach (var item in students)
            {
                            
                PdfPCell cell2 = new PdfPCell(new Phrase("Navn"));
                cell2.BackgroundColor = new BaseColor(198, 198, 198);
                PdfPCell cell3 = new PdfPCell(new Phrase("Hold"));
                cell3.BackgroundColor = new BaseColor(198, 198, 198);
                PdfPCell cell4 = new PdfPCell(new Phrase("DB Bruger"));
                cell4.BackgroundColor = new BaseColor(198, 198, 198);
                PdfPCell cell5 = new PdfPCell(new Phrase("DB PW"));
                cell5.BackgroundColor = new BaseColor(198, 198, 198);
                PdfPCell cell6 = new PdfPCell(new Phrase("FTP Bruger"));
                cell6.BackgroundColor = new BaseColor(198, 198, 198);
                PdfPCell cell7 = new PdfPCell(new Phrase("FTP PW"));
                cell7.BackgroundColor = new BaseColor(198, 198, 198);
                PdfPCell cell8 = new PdfPCell(new Phrase("Website Port"));
                cell8.BackgroundColor = new BaseColor(198, 198, 198);

                table.AddCell(cell2);
                table.AddCell(cell3);
                table.AddCell(cell4);
                table.AddCell(cell5);
                table.AddCell(cell6);
                table.AddCell(cell7);
                table.AddCell(cell8);
                table.AddCell(item.Name);
                table.AddCell(item.Team.Name);
                table.AddCell(item.Credentials.DatabaseUserName);
                table.AddCell(item.Credentials.DatabasePassword);
                table.AddCell(item.Credentials.FTPUserName);
                table.AddCell(item.Credentials.FTPPassword);
                table.AddCell(item.Credentials.WebsitePort + "");
            }

            doc.Open();
            doc.Add(para);
            doc.Add(table);
            doc.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }
    }
}
