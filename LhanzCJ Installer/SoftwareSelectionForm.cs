using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LhanzCJ_Installer
{
    public partial class SoftwareSelectionForm : Form
    {
        private List<CustomSoftware> customSoftwareList = new List<CustomSoftware>();
        private readonly string jsonPath = "additional_software.json";

        public SoftwareSelectionForm()
        {
            InitializeComponent();
            LoadCustomSoftware();
        }

        private void LoadCustomSoftware()
        {
            string json = File.ReadAllText(jsonPath);
            if (string.IsNullOrWhiteSpace(json))
            {
                customSoftwareList = new List<CustomSoftware>();
            }
            else
            {
                customSoftwareList = JsonConvert.DeserializeObject<List<CustomSoftware>>(json);
            }
            checkedListBox.Items.AddRange(customSoftwareList.Select(s => s.Name).ToArray());
        }

        private void btnAddNew_Click(object sender, EventArgs e)
        {
            using (var addForm = new AddSoftwareForm())
            {
                if (addForm.ShowDialog() == DialogResult.OK)
                {
                    var newSoftware = new CustomSoftware
                    {
                        Name = addForm.SoftwareName,
                        Url = addForm.SoftwareUrl
                    };
                    customSoftwareList.Add(newSoftware);
                    checkedListBox.Items.Add(newSoftware.Name, true);
                    SaveCustomSoftware();
                }
            }
        }

        private void SaveCustomSoftware()
        {
            string json = JsonConvert.SerializeObject(customSoftwareList, Formatting.Indented);
            File.WriteAllText(jsonPath, json);
        }

        public List<CustomSoftware> GetSelectedSoftware()
        {
            return checkedListBox.CheckedIndices
                .Cast<int>()
                .Select(i => customSoftwareList[i])
                .ToList();
        }
    }
}
