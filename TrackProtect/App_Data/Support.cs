using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace TrackProtect
{
    public class RegisterInfo
    {
        public long RegisterId { get; set; }
        public byte[] Certificate { get; set; }
        public DateTime RegistrationDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public long UserId { get; set; }
    }

    public class DocumentInfo
    {
        public long DocumentId { get; set; }
        public long RegisterId { get; set; }
        public string DocumentName { get; set; }
        public string DocumentHash { get; set; }
    }

    public class ClientInfo
    {
        public long ClientId { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string ZipCode { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Language { get; set; }
        public string Telephone { get; set; }
        public string Cellular { get; set; }
        public string CompanyName { get; set; }
        public long UserId { get; set; }
        public int SubscriptionType { get; set; }
        public int Credits { get; set; }
        public string UserUid { get; set; }
        public int WhmcsClientId { get; set; }
        public string AccountOwner { get; set; }
        public string BumaCode { get; set; }
        public string SenaCode { get; set; }
        public string IsrcCode { get; set; }
        public string TwitterId { get; set; }
        public string FacebookId { get; set; }
        public string SoundCloudId { get; set; }
        public string SoniallId { get; set; }
        public string OwnerKind { get; set; }
        public string CreditCardNr { get; set; }
        public string CreditCardCvv { get; set; }
        public string EmailReceipt { get; set; }
        public string Referer { get; set; }
        public char Gender { get; set; }
        public DateTime Birthdate { get; set; }
        public string stagename { get; set; }

        public ClientInfo()
        {
            ClientId = -1L;
            LastName = string.Empty;
            FirstName = string.Empty;
            AddressLine1 = string.Empty;
            AddressLine2 = string.Empty;
            ZipCode = string.Empty;
            State = string.Empty;
            City = string.Empty;
            Country = string.Empty;
            Language = string.Empty;
            Telephone = string.Empty;
            Cellular = string.Empty;
            CompanyName = string.Empty;
            UserId = -1L;
            SubscriptionType = -1;
            Credits = 0;
            UserUid = string.Empty;
            WhmcsClientId = -1;
            AccountOwner = string.Empty;
            BumaCode = string.Empty;
            SenaCode = string.Empty;
            IsrcCode = string.Empty;
            TwitterId = string.Empty;
            FacebookId = string.Empty;
            SoundCloudId = string.Empty;
            SoniallId = string.Empty;
            OwnerKind = string.Empty;
            CreditCardNr = string.Empty;
            CreditCardCvv = string.Empty;
            EmailReceipt = string.Empty;
            Referer = string.Empty;
            Gender = 'U';
            Birthdate = new DateTime(1, 1, 1, 0, 0, 0, 0);
            stagename = string.Empty;
        }

        public string GetFullName()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(FirstName);
            sb.Append(' ');
            sb.Append(LastName);
            return sb.ToString();
        }
    }

    public class UserState
    {
        public long UserId { get; set; }
        public int State { get; set; }

        public UserState()
        {
            UserId = -1L;
            State = -1;
        }
    }

    public class UserInfo
    {
        public long UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public short IsApproved { get; set; }
        public bool IsLockedOut { get; set; }
        public int Credits { get; set; }
        public string Password { get; set; }
        public string UserUid { get; set; }
        public DateTime MemberSince { get; set; }
        public int SubscriptionType { get; set; }
        public string Comment { get; set; }
        //Added by Nagesh for Active flag.
        public int IsActive { get; set; }

        public UserInfo()
        {
            UserId = -1L;
            UserName = string.Empty;
            Email = string.Empty;
            IsApproved = 1;
            IsLockedOut = false;
            Password = string.Empty;
            Credits = 0;
            UserUid = string.Empty;
            MemberSince = DateTime.MinValue;
            SubscriptionType = -1;
            Comment = string.Empty;
        }
    }

    public class Product
    {
        public string ProductPlan { get; set; }
        public string Credits { get; set; }
        public string ProductDesc { get; set; }
        public string ProductPrice { get; set; }
        public long ProductId { get; set; }
        public string Extra { get; set; }
        public string Price { get; set; }

    }

    public class ProductInfo
    {
        public long ProductId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Credits { get; set; }
        public string Extra { get; set; }
        public string ProductPlan { get; set; }

        public ProductInfo()
        {
            ProductId = -1L;
            Name = string.Empty;
            Description = string.Empty;
            Credits = 0;
            Extra = string.Empty;
            ProductPlan = string.Empty;
        }

        public ProductInfo(long productId, string name, string description, int credits, string extra)
        {
            ProductId = productId;
            Name = name;
            Description = description;
            Credits = credits;
            Extra = extra;
        }

        public ProductInfo(long productId, string name, string description, int credits, string extra, string productPlan)
        {
            ProductId = productId;
            Name = name;
            Description = description;
            Credits = credits;
            Extra = extra;
            ProductPlan = productPlan;
        }
    }

    public class ProductInfoList : List<ProductInfo>
    {
    }

    public class ProductPriceInfo
    {
        public long ProductPriceId { get; set; }
        public decimal Price { get; set; }
        public string IsoCurrency { get; set; }
        public string Iso2Country { get; set; }
        public long ProductId { get; set; }

        public ProductPriceInfo()
        {
            ProductPriceId = -1L;
            Price = 0m;
            IsoCurrency = string.Empty;
            Iso2Country = string.Empty;
            ProductId = -1L;
        }

        public ProductPriceInfo(long productPriceId, decimal price, string isoCurrency, string iso2Country, long productId)
        {
            ProductPriceId = productPriceId;
            Price = price;
            IsoCurrency = isoCurrency;
            Iso2Country = iso2Country;
            ProductId = productId;
        }
    }

    public class ProductPriceInfoList : List<ProductPriceInfo>
    {
    }

    public class CreditHistory
    {
        public long CreditHistoryId { get; set; }
        public long UserId { get; set; }
        public long ProductId { get; set; }
        public int Credits { get; set; }
        public DateTime PurchaseDate { get; set; }
        public bool Expired { get; set; }
        public string Description { get; set; }
        public string InvoiceFile { get; set; }
        public long TransactionId { get; set; }

        public CreditHistory(long creditHistoryId, long userId, long productId, int credits, DateTime purchaseDate, bool expired, long transactionId)
        {
            CreditHistoryId = creditHistoryId;
            UserId = userId;
            ProductId = productId;
            Credits = credits;
            PurchaseDate = purchaseDate;
            Expired = expired;
            TransactionId = transactionId;
        }

        public CreditHistory(long creditHistoryId, long userId, long productId, int credits, DateTime purchaseDate, int expired, long transactionId) :
            this(creditHistoryId, userId, productId, credits, purchaseDate, (expired > 0), transactionId)
        {
        }
    }

    public class CreditHistoryList : List<CreditHistory>
    {
    }

    public class Transaction
    {
        public long TransactionId { get; set; }
        public long UserId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public long ProductId { get; set; }
        public string Status { get; set; }
        public string StatusCode { get; set; }
        public string Merchant { get; set; }
        public string PaymentId { get; set; }
        public string TransactionReference { get; set; }
        public string PaymentMethod { get; set; }
        public TransactionLines TransactionLines { get; set; }

        public decimal TotalAmount
        {
            get
            {
                decimal total = 0m;
                foreach (TransactionLine tl in TransactionLines)
                {
                    total += (tl.Price * tl.Quantity);
                }
                return total;
            }
        }

        public decimal TotalVatAmount
        {
            get
            {
                decimal total = 0m;
                foreach (TransactionLine tl in TransactionLines)
                {
                    decimal totAmt = (tl.Price * tl.Quantity);
                    decimal totVat = totAmt * tl.VatPercentage;
                    decimal totDif = totVat - totAmt;
                    total += totDif;
                }
                return total;
            }
        }

        public Transaction()
        {
            TransactionLines = new TransactionLines();
        }

        public decimal GetVatAmount(decimal vatPercentage)
        {
            decimal total = 0m;
            foreach (TransactionLine tl in TransactionLines)
            {
                if (tl.VatPercentage != vatPercentage)
                    continue;

                decimal totAmt = (tl.Price * tl.Quantity);
                decimal totVat = totAmt * tl.VatPercentage;
                decimal totDif = totVat - totAmt;
                total += totDif;
            }
            return total;
        }
    }

    public class TransactionLine
    {
        public int ItemLine { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal VatPercentage { get; set; }
        public decimal VatAmount { get; set; }
    }

    public class TransactionLines : List<TransactionLine>
    {

    }

    public class Transactions : List<Transaction>
    {

    }

    public class Vat
    {
        public long VatId { get; set; }
        public string Culture { get; set; }
        public decimal Percentage { get; set; }
        public string Name { get; set; }
    }

    public class VatList : List<Vat>
    {

    }
}

