using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Security.Cryptography;
using System.Threading;
using System.Web.Security;
using System.Web.UI.HtmlControls;
using BodyArchitect.Service.V2;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Payments;
using BodyArchitect.Settings;
using ASP;

public partial class V2_Payments : System.Web.UI.Page
{
    protected string ProfileId { get; set; }
    protected string PayPalUrl { get; set; }
    protected string PayPalButton30 { get; set; }
    protected string PayPalButton120 { get; set; }
    protected string PayPalNotifyUrl { get; set; }
    protected string PowrotTransferujUrl { get; set; }

    protected string AmazonUrl { get; set; }
    protected string AmazonNotifyUrl { get; set; }
    protected string AmazonAccessKey { get; set; }
    protected string AmazonSecretKey { get; set; }
    protected string AmazonSignature { get; set; }
    public string payNowWidgetForm { get; set; }

    protected string TransferujUrl { get; set; }
    protected string TransferujNotifyUrl { get; set; }
    protected string TransferujId { get; set; }
    protected string TransferujKwota { get; set; }
    protected string TransferujCrc { get; set; }
    protected string TransferujOpis { get; set; }
    protected string TransferujKodPotwierdzajcy { get; set; }
    protected string TransferujMd5 { get; set; }
    protected string Transferuj120Md5 { get; set; }
    protected string Transferuj120Kwota { get; set; }
    protected string Transferuj120Opis { get; set; }
    protected string Transferuj120Crc { get; set; }

    protected string Przelewy24Url { get; set; }
    protected string Przelewy24NotifyUrl { get; set; }
    protected string Przelewy24Id { get; set; }
    protected string Przelewy24Kwota { get; set; }
    protected string Przelewy24Kwota120 { get; set; }
    protected string Przelewy24Crc { get; set; }
    protected string Przelewy24Crc120 { get; set; }
    protected string Przelewy24SessionId { get; set; }
    protected string Przelewy24SessionId120 { get; set; }
    protected string Language { get; set; }
    protected string ProfileEmail { get; set; }
    protected bool UseTransferuj { get; set; }

    protected void Page_PreInit(object sender, EventArgs e)
    {
        Thread.CurrentThread.CurrentUICulture = new CultureInfo("pl");
        Thread.CurrentThread.CurrentCulture = new CultureInfo("pl");
        UseTransferuj = false;
    }

    void hideControls()
    {
        przelewyFooter.Visible = przelewy30Form.Visible = przelewy120Form.Visible = !UseTransferuj;
        transferujFooter.Visible=transferuj30Form.Visible = transferuj120Form.Visible = UseTransferuj;
        
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        PayPalHandler payPal = new PayPalHandler(InternalBodyArchitectService.PaymentsManager);

        PowrotTransferujUrl = ApplicationSettings.ServerUrl + "V2/PaymentsFinished.aspx?Transferuj=1";
        PayPalNotifyUrl = ApplicationSettings.ServerUrl + "V2/PayPalOrderProcessing.aspx";
        PayPalUrl = payPal.PayPalUrl;
        PayPalButton30 = payPal.BAPoints_30_Button;
        PayPalButton120 = payPal.BAPoints_120_Button;
        //AmazonHandler amazon = new AmazonHandler(InternalBodyArchitectService.PaymentsManager);
        //AmazonNotifyUrl = ApplicationSettings.ServerUrl + "V2/AmazonOrderProcessing.aspx";
        //AmazonNotifyUrl = "http://test.bodyarchitectonline.com/V2/AmazonOrderProcessing.aspx";
        //amazon.AmazonNotifyUrl = AmazonNotifyUrl;
        //AmazonUrl = amazon.AmazonUrl;
        //TODO: Amazon credentials
        //AmazonAccessKey = "AKIAIPL5YHFX7JYBMJTA";   //obtain keys at https://portal.aws.amazon.com/gp/aws/securityCredentials
        //AmazonSecretKey = "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";
                
        MyRenderForm.RenderFormTag = false;
        
#if !DEBUG
        if (Request.QueryString["Token"] == null)
        {
            Response.Redirect("http://bodyarchitectonline.com");
        }
#else
        ProfileEmail = "test@email.com";
#endif
        ////now using specified token we must check if this token is authenticated and if yes then we take profile id
        if (Request.QueryString["Token"] != null)
        {
            Token token = new Token(new Guid(Request.QueryString["Token"]));
            var securityInfo = InternalBodyArchitectService.SecurityManager.EnsureAuthentication(token);
            ProfileId = securityInfo.SessionData.Profile.GlobalId.ToString();
            ProfileEmail = securityInfo.SessionData.Profile.Email;
            lblUserName.Text = securityInfo.SessionData.Profile.UserName;
            Language = securityInfo.SessionData.Token.Language;
        }
        //payNowWidgetForm = ButtonGenerator.GenerateForm(AmazonAccessKey, AmazonSecretKey, "USD 5", "BAPoints_30",
        //    ProfileId, "0", null, null, "1", AmazonNotifyUrl, "0", "HmacSHA256", amazon.UseSandbox ? "sandbox" : "prod");

        TransferujHandler transferuj = new TransferujHandler(InternalBodyArchitectService.PaymentsManager);
        TransferujUrl = transferuj.Url;
        TransferujNotifyUrl = ApplicationSettings.ServerUrl + "V2/TransferujOrderProcessing.aspx";
        TransferujId = transferuj.TransferujId;
        TransferujKwota = "15";
        TransferujCrc = string.Format("{0}|{1}",ProfileId,"BAPoints_30");
        TransferujKodPotwierdzajcy = transferuj.TransferujKodPotwierdzajcy;

        Transferuj120Opis = (string)this.GetGlobalResourceObject("Payments.aspx", "lbl120PointsPaymentDescription.value");
        TransferujOpis = (string)this.GetGlobalResourceObject("Payments.aspx", "lbl30PointsPaymentDescription.value");
        TransferujMd5 = FormsAuthentication.HashPasswordForStoringInConfigFile(
            TransferujId + TransferujKwota + TransferujCrc + TransferujKodPotwierdzajcy, "MD5").ToLower();

        Transferuj120Kwota = "60";
        Transferuj120Crc = string.Format("{0}|{1}", ProfileId, "BAPoints_120");
        Transferuj120Md5 = FormsAuthentication.HashPasswordForStoringInConfigFile(
            TransferujId + Transferuj120Kwota + Transferuj120Crc + TransferujKodPotwierdzajcy, "MD5").ToLower();



        Przelewy24Handler przelewy24 = new Przelewy24Handler(InternalBodyArchitectService.PaymentsManager);
        Przelewy24Id = przelewy24.MyId;
        Przelewy24NotifyUrl = przelewy24.NotifyUrl;
        Przelewy24Url = przelewy24.Url;
        Przelewy24Kwota = "1500";
        Przelewy24Kwota120 = "6000";
        Przelewy24SessionId = string.Format("{0}|{1}|BAPoints_30", Guid.NewGuid().ToString(), ProfileId);
        Przelewy24SessionId120 = string.Format("{0}|{1}|BAPoints_120", Guid.NewGuid().ToString(), ProfileId);
        string value = Przelewy24SessionId + "|" + Przelewy24Id + "|" + Przelewy24Kwota + "|" + przelewy24.KluczCRC;
        string value120 = Przelewy24SessionId120 + "|" + Przelewy24Id + "|" + Przelewy24Kwota120 + "|" + przelewy24.KluczCRC;
        Przelewy24Crc = FormsAuthentication.HashPasswordForStoringInConfigFile(value, "MD5").ToLower();
        Przelewy24Crc120 = FormsAuthentication.HashPasswordForStoringInConfigFile(value120, "MD5").ToLower();


        hideControls();
    }

    
}