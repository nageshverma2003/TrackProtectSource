using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Web;

namespace TrackProtect
{
    internal class InvoiceForm
    {
        string _templateFilename = string.Empty;

        public string[] ClientAddress { get; set; }
        public string InvoiceNumber { get; set; }
        public string InvoiceDate { get; set; }
        public PdfInvoiceLines InvoiceLines { get; set; }
        public float DesiredTableHeight { get; set; }
        public string CompanyName { get; set; }

        public InvoiceForm(string templateFilename)
        {
            ClientAddress = new string[5];
            InvoiceLines = new PdfInvoiceLines();
            DesiredTableHeight = 300f;

            //string workFilename = HttpContext.Current.Server.MapPath(templateFilename);
            string workFilename = templateFilename;
            if (!File.Exists(workFilename))
                throw new FileNotFoundException("File doesn't exist.", workFilename);

            _templateFilename = workFilename;
        }

        public void GenerateInvoice(string targetFilename, string companyName)
        {
            CultureInfo culture = new CultureInfo("nl-NL");
            PdfReader reader = new PdfReader(_templateFilename);
            using (PdfStamper stamper = new PdfStamper(reader, new FileStream(targetFilename, FileMode.Create)))
            {
                AcroFields form = stamper.AcroFields;
                var fieldKeys = form.Fields.Keys;
                foreach (string fieldKey in fieldKeys)
                {
                    if (string.IsNullOrEmpty(companyName))
                    {
                        if (fieldKey.Contains("clientaddress1") && !string.IsNullOrEmpty(ClientAddress[0]))
                            form.SetField(fieldKey, ClientAddress[0]);
                        if (fieldKey.Contains("clientaddress2") && !string.IsNullOrEmpty(ClientAddress[1]))
                            form.SetField(fieldKey, ClientAddress[1]);
                        if (fieldKey.Contains("clientaddress3") && !string.IsNullOrEmpty(ClientAddress[2]))
                            form.SetField(fieldKey, ClientAddress[2]);
                        if (fieldKey.Contains("clientaddress4") && !string.IsNullOrEmpty(ClientAddress[3]))
                            form.SetField(fieldKey, ClientAddress[3]);
                        if (fieldKey.Contains("clientaddress5") && !string.IsNullOrEmpty(ClientAddress[4]))
                            form.SetField(fieldKey, ClientAddress[4]);
                    }
                    else
                    {
                        if (fieldKey.Contains("clientaddress1") && !string.IsNullOrEmpty(ClientAddress[0]))
                            form.SetField(fieldKey, companyName);
                        if (fieldKey.Contains("clientaddress2") && !string.IsNullOrEmpty(ClientAddress[1]))
                            form.SetField(fieldKey, ClientAddress[0]);
                        if (fieldKey.Contains("clientaddress3") && !string.IsNullOrEmpty(ClientAddress[2]))
                            form.SetField(fieldKey, ClientAddress[1] + " " + ClientAddress[2]);
                        if (fieldKey.Contains("clientaddress4") && !string.IsNullOrEmpty(ClientAddress[3]))
                            form.SetField(fieldKey, ClientAddress[3]);
                        if (fieldKey.Contains("clientaddress5") && !string.IsNullOrEmpty(ClientAddress[4]))
                            form.SetField(fieldKey, ClientAddress[4]);
                    }

                    if (fieldKey.Contains("invoicenumber"))
                        form.SetField(fieldKey, InvoiceNumber);
                    if (fieldKey.Contains("invoicedate"))
                        form.SetField(fieldKey, InvoiceDate);
                }

                PdfContentByte pcb = stamper.GetUnderContent(1);

                // Prepare the table
                PdfPTable table = new PdfPTable(5);
                table.TotalWidth = 520f;
                table.SetWidths(new float[] { 10f, 60f, 10f, 10f, 10f });

                // Add the header cells
                AddHeaderCell(table, Resources.Resource.ItemNo, Rectangle.BOTTOM_BORDER | Rectangle.TOP_BORDER);
                AddHeaderCell(table, Resources.Resource.Description, Rectangle.BOTTOM_BORDER | Rectangle.TOP_BORDER);
                AddHeaderCell(table, Resources.Resource.Quantity, Rectangle.BOTTOM_BORDER | Rectangle.TOP_BORDER);
                AddHeaderCell(table, Resources.Resource.Price, Rectangle.BOTTOM_BORDER | Rectangle.TOP_BORDER);
                AddHeaderCell(table, Resources.Resource.Amount, Rectangle.BOTTOM_BORDER | Rectangle.TOP_BORDER);

                decimal subtotal = 0m, total = 0m, vat = 0m;
                Dictionary<decimal, VatInfo> vatInfos = new Dictionary<decimal, VatInfo>();
                if (InvoiceLines.Count > 0)
                {
                    int item = 1;
                    foreach (PdfInvoiceLine il in InvoiceLines)
                    {
                        if (!vatInfos.ContainsKey(il.VatRate))
                            vatInfos.Add(il.VatRate, new VatInfo() { VatRate = il.VatRate, Total = 0 });

                        decimal vatRate = 1 + (il.VatRate / 100);
                        decimal unitPrice = (il.UnitPrice / vatRate);
                        decimal vatAmount = (il.UnitPrice - unitPrice) * il.Quantity;
                        decimal amount = il.Quantity * unitPrice;

                        // Sum all VAT amounts per VAT rate
                        vatInfos[il.VatRate].Total += vatAmount;

                        // Push information to the document
                        AddDataCell(table, item.ToString(), 0);
                        AddDataCell(table, il.Description, 0);
                        AddDataCell(table, il.Quantity.ToString(), 0);
                        AddDataCell(table, string.Format(culture, "{0:C}", unitPrice), 0);
                        AddDataCell(table, string.Format(culture, "{0:C}", amount), 0);

                        // Calculate intermediate totals
                        subtotal += amount;
                        vat += vatAmount;
                        total = subtotal + vat;

                        ++item;
                    }

                    AddPadding(table);

                    AddTotalLine(table, Resources.Resource.Subtotal, string.Format(culture, "{0:C}", subtotal), Rectangle.TOP_BORDER);
                    foreach (decimal vatRateKey in vatInfos.Keys)
                    {
                        string title = string.Format("{0} {1:N0}%", Resources.Resource.VAT, vatRateKey);
                        AddTotalLine(table, title, string.Format(culture, "{0:C}", vat), Rectangle.NO_BORDER);
                    }
                    AddTotalLine(table, Resources.Resource.Total, string.Format(culture, "{0:C}", total), Rectangle.BOTTOM_BORDER);
                }

                table.WriteSelectedRows(0, table.Rows.Count, 40f, 500f, pcb);

                stamper.FormFlattening = true;
            }
        }

        private void AddHeaderCell(PdfPTable table, string text, int border)
        {
            PdfPCell cell = new PdfPCell(new Phrase(text, new Font(Font.FontFamily.HELVETICA, 10f, Font.BOLD)));
            cell.Border = border;
            cell.BorderColorTop = BaseColor.BLACK;
            cell.BorderWidthTop = 0f;            
            cell.BorderColorBottom = BaseColor.BLACK;
            cell.BorderWidthBottom = 1f;
            table.AddCell(cell);
        }

        private void AddDataCell(PdfPTable table, string text, int alignment)
        {
            PdfPCell cell = new PdfPCell(new Phrase(text, new Font(Font.FontFamily.HELVETICA, 10f, Font.NORMAL)));
            cell.Border = 0;
            cell.HorizontalAlignment = alignment;
            table.AddCell(cell);
        }

        private void AddPadding(PdfPTable table)
        {
            while (table.CalculateHeights() < DesiredTableHeight)
            {
                AddDataCell(table, " ", 0);
                AddDataCell(table, " ", 0);
                AddDataCell(table, " ", 0);
                AddDataCell(table, " ", 0);
                AddDataCell(table, " ", 0);
            }
        }

        private void AddTotalLine(PdfPTable table, string text, string value, int border)
        {
            PdfPCell cell = new PdfPCell();
            cell.Colspan = 3;
            cell.BorderColorTop = BaseColor.WHITE;
            cell.BorderColorBottom = BaseColor.WHITE;
            cell.Border = border;
            if ((border & Rectangle.TOP_BORDER) != 0)
            {
                cell.BorderColorTop = BaseColor.BLACK;
                cell.BorderWidthTop = 1f;
            }
            if ((border & Rectangle.BOTTOM_BORDER) != 0)
            {
                cell.BorderColorBottom = BaseColor.BLACK;
                cell.BorderWidthBottom = 0f;
            }
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(text, new Font(Font.FontFamily.HELVETICA, 10f, Font.BOLD)));
            cell.HorizontalAlignment = 2;
            cell.BorderColorTop = BaseColor.WHITE;
            cell.BorderColorBottom = BaseColor.WHITE;
            cell.Border = border;
            if ((border & Rectangle.TOP_BORDER) != 0)
            {
                cell.BorderColorTop = BaseColor.BLACK;
                cell.BorderWidthTop = 1f;
            }
            if ((border & Rectangle.BOTTOM_BORDER) != 0)
            {
                cell.BorderColorBottom = BaseColor.BLACK;
                cell.BorderWidthBottom = 0f;
            }
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(value, new Font(Font.FontFamily.HELVETICA, 10f, Font.NORMAL)));
            cell.HorizontalAlignment = 2;
            cell.BorderColorTop = BaseColor.WHITE;
            cell.BorderColorBottom = BaseColor.WHITE;
            cell.Border = border;
            if ((border & Rectangle.TOP_BORDER) != 0)
            {
                cell.BorderColorTop = BaseColor.BLACK;
                cell.BorderWidthTop = 1f;
            }
            if ((border & Rectangle.BOTTOM_BORDER) != 0)
            {
                cell.BorderColorBottom = BaseColor.BLACK;
                cell.BorderWidthBottom = 0f;
            }
            table.AddCell(cell);
        }

        class VatInfo
        {
            public decimal VatRate { get; set; }
            public decimal Total { get; set; }
        }
    }

    internal class PdfInvoiceLine
    {
        public string Description { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal VatRate { get; set; }
    }

    internal class PdfInvoiceLines : List<PdfInvoiceLine>
    {
    }
}
