using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace SEMD
{
    public partial class Form1 : Form
    {
        private string _xmlStr = string.Empty;

        public Form1()
        {
            InitializeComponent();
        }

        private void buttonGenerate_Click(object sender, EventArgs e)
        {
            var arg = new List<string>() { "GUID" };
            var arg2 = new List<string>() { "@emdTypeGuid" };
            var arg3 = new List<string>();


            string script = $@"begin transaction

declare @xsltTransformation varchar(max) = '{this._xmlStr}';

";

            script += "declare\n";


            if (this.textBoxGuid.Text.Length > 0)
            {
                script += $"@emdTypeGuid uniqueidentifier = '{this.textBoxGuid.Text}',\n";
            }
            else
            {
                script += $"@emdTypeGuid uniqueidentifier = '{Guid.NewGuid()}',\n";
            }

            if (this.textBoxCode.Text.Length > 0)
            {
                script += $"@emdTypeCode varchar(3) = '{this.textBoxCode.Text}',\n";
                arg.Add("Code");
                arg2.Add("@emdTypeCode");
                arg3.Add("Code                  = @emdTypeCode");
            }

            if (this.textBoxName.Text.Length > 0)
            {
                script += $"@emdTypeName varchar(250) = '{this.textBoxName.Text}',\n";
                arg.Add("Name");
                arg2.Add("@emdTypeName");
                arg3.Add("Name                  = @emdTypeName");
            }


            if (this.textBoxIsSemd.Text.Length > 0)
            {
                script += $"@isSemd bit = {this.textBoxIsSemd.Text},\n";
                arg.Add("IsSemd");
                arg2.Add("@isSemd");
                arg3.Add("IsSemd                = @isSemd");
            }

            if (this.textBoxSourceGuid.Text.Length > 0)
            {
                script += $"@sourceId int = isnull((select top 1 EmdTypeSourceId from oms_EmdTypeSource where EmdTypeSourceGuid = '{this.textBoxSourceGuid.Text}'), 0),\n";
                arg.Add("rf_EmdTypeSourceID");
                arg2.Add("@sourceId");
                arg3.Add("rf_EmdTypeSourceID    = @sourceId");
            }

            if (this.textBoxMdtGuid.Text.Length > 0)
            {
                script += $"@medDocumentTypeId int = isnull((select top 1 kl_MedDocumentTypeID from oms_kl_MedDocumentType where MedDocumentTypeGUID = '{this.textBoxMdtGuid.Text}'), 0),\n";
                arg.Add("rf_MedDocumentTypeID");
                arg2.Add("@medDocumentTypeId");
                arg3.Add("rf_MedDocumentTypeID  = @medDocumentTypeId");
            }

            if (this.textBoxIsEmdReq.Text.Length > 0)
            {
                script += $"@isEmdRequest bit = {this.textBoxIsEmdReq.Text},\n";
                arg.Add("isEmdRequest");
                arg2.Add("@isEmdRequest");
                arg3.Add("isEmdRequest          = @isEmdRequest");
            }

            if (this.textBoxReqDocTypeDefGuid.Text.Length > 0)
            {
                script += $"@requestDocTypeDefGuid uniqueidentifier = '{this.textBoxReqDocTypeDefGuid.Text}'\n";
                arg.Add("RequestDocTypeDefGUID");
                arg2.Add("@requestDocTypeDefGuid");
                arg3.Add("RequestDocTypeDefGUID = @requestDocTypeDefGuid");
            }
         

            script += $@"
if (not exists(select 1 from oms_EmdType where GUID = @emdTypeGuid))
begin
	insert into oms_EmdType ({string.Join(", ", arg)})
	values ({string.Join(", ", arg2)})
end
else
begin
	update oms_EmdType
	set {string.Join(",\n\t\t", arg3)}
	where GUID = @emdTypeGuid
end";

            this.richTextBoxScript.Text = script;
        }

        private void AddDeclare(string paramName, string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                this.richTextBoxScript.Text += $"{paramName} = '{value}',\n";
            }
        }

        private void buttonLoadXlt_Click(object sender, EventArgs e)
        {
            if (this.openFileDialog1.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }

            this.labelLoadXml.Text = System.IO.Path.GetFileNameWithoutExtension(this.openFileDialog1.FileName);

            this._xmlStr = File.ReadAllText(this.openFileDialog1.FileName).Replace("'", "''");

            // получаем выбранный файл
            //string filename = openFileDialog1.FileName;
            // сохраняем текст в файл
            //System.IO.File.WriteAllText(filename, textBox1.Text);
            //MessageBox.Show("Файл сохранен");
        }
    }
}
