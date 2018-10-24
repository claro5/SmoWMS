using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Smobiler.Core;
using Smobiler.Core.Controls;
using SMOWMS.DTOs.InputDTO;
using SMOWMS.Domain.Entity;
using System.Data;
using SMOWMS.DTOs.Enum;
using SMOWMS.DTOs.OutputDTO;

namespace SMOWMS.UI.AssetsManager
{
    partial class frmAssTransferDetail : Smobiler.Core.Controls.MobileForm
    {
        #region "definition"
        AutofacConfig autofacConfig = new AutofacConfig();     //����������
        public String TOID;     //���������
        #endregion
        /// <summary>
        /// ҳ���ʼ��
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmTransferDetailSN_Load(object sender, EventArgs e)
        {
            Bind();
        }
        /// <summary>
        /// ��������
        /// </summary>
        public void Bind()
        {
            try
            {
                TOInputDto TOData = autofacConfig.assTransferOrderService.GetByID(TOID);
                coreUser DeanInUser = autofacConfig.coreUserService.GetUserByID(TOData.MANAGER);
                coreUser DealUser = autofacConfig.coreUserService.GetUserByID(TOData.HANDLEMAN);
                WHStorageLocationOutputDto whLoc = autofacConfig.wareHouseService.GetSLByID(TOData.WAREID, TOData.STID, TOData.DESSLID);
                lblTDInMan.Text = DeanInUser.USER_NAME;
                lblDealMan.Text = DealUser.USER_NAME;
                lblLocation.Text = whLoc.WARENAME + "/" + whLoc.STNAME + "/" + whLoc.SLNAME;
                DatePicker.Value = TOData.TRANSFERDATE;
                if (String.IsNullOrEmpty(TOData.NOTE) == false) lblNote.Text = TOData.NOTE;

                DataTable tableAssets = new DataTable();      //δ����SN���ʲ��б�
                tableAssets.Columns.Add("ASSID");             //�ʲ����
                tableAssets.Columns.Add("NAME");              //�ʲ�����
                tableAssets.Columns.Add("IMAGE");             //�ʲ�ͼƬ
                tableAssets.Columns.Add("SN");                //���к�
                tableAssets.Columns.Add("STATUS");            //����״̬
                foreach (AssTransferOrderRow Row in TOData.Rows)
                {
                    Assets assets = autofacConfig.orderCommonService.GetAssetsByID(Row.ASSID);
                    if (Row.STATUS == 0)
                    {
                        tableAssets.Rows.Add(Row.ASSID, assets.NAME, assets.IMAGE, Row.SN, "������");
                    }
                    else if (Row.STATUS == 1)
                    {
                        tableAssets.Rows.Add(Row.ASSID, assets.NAME, assets.IMAGE, Row.SN, "�������");
                    }
                    else
                    {
                        tableAssets.Rows.Add(Row.ASSID, assets.NAME, assets.IMAGE, Row.SN, "����ȡ��");
                    }
                }
                if (tableAssets.Rows.Count > 0)
                {
                    ListAssetsSN.DataSource = tableAssets;
                    ListAssetsSN.DataBind();
                }
                if (Client.Session["Role"].ToString() == "SMOWMSUser") plButton.Visible = false;
                //���ά�޵�����ɣ�������ά�޵�������ť
                if (TOData.STATUS == 1 || TOData.STATUS==2) plButton.Visible = false;
            }
            catch (Exception ex)
            {
                Toast(ex.Message);
            }
        }
        /// <summary>
        /// ����ȷ��
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSure_Press(object sender, EventArgs e)
        {
            frmAssTransferDeal frm = new frmAssTransferDeal();
            frm.TOID = TOID;
            frm.Type = PROCESSMODE.����ȷ��;
            Show(frm, (MobileForm sender1, object args) =>
            {
                if (frm.ShowResult == ShowResult.Yes)
                    Bind();   //ˢ��������ʾ
            });
        }
        /// <summary>
        /// ����ȡ��
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Press(object sender, EventArgs e)
        {
            frmAssTransferDeal frm = new frmAssTransferDeal();
            frm.TOID = TOID;
            frm.Type = PROCESSMODE.����ȡ��;
            Show(frm, (MobileForm sender1, object args) =>
            {
                if (frm.ShowResult == ShowResult.Yes)
                    Bind();   //ˢ��������ʾ
            });
        }
    }
}