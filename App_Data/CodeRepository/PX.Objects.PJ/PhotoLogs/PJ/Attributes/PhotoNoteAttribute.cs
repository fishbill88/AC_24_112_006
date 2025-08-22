/* ---------------------------------------------------------------------*
*                             Acumatica Inc.                            *

*              Copyright (c) 2005-2024 All rights reserved.             *

*                                                                       *

*                                                                       *

* This file and its contents are protected by United States and         *

* International copyright laws.  Unauthorized reproduction and/or       *

* distribution of all or any portion of the code contained herein       *

* is strictly prohibited and will result in severe civil and criminal   *

* penalties.  Any violations of this copyright will be prosecuted       *

* to the fullest extent possible under law.                             *

*                                                                       *

* UNDER NO CIRCUMSTANCES MAY THE SOURCE CODE BE USED IN WHOLE OR IN     *

* PART, AS THE BASIS FOR CREATING A PRODUCT THAT PROVIDES THE SAME, OR  *

* SUBSTANTIALLY THE SAME, FUNCTIONALITY AS ANY ACUMATICA PRODUCT.       *

*                                                                       *

* THIS COPYRIGHT NOTICE MAY NOT BE REMOVED FROM THIS FILE.              *

* --------------------------------------------------------------------- */

using System;
using System.Collections.Generic;
using System.Linq;
using PX.Objects.PJ.PhotoLogs.Descriptor;
using PX.Objects.PJ.PhotoLogs.PJ.DAC;
using PX.Common;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.Common.Extensions;
using PX.SM;

namespace PX.Objects.PJ.PhotoLogs.PJ.Attributes
{
    public class PhotoNoteAttribute : PXNoteAttribute
    {
        public override void noteFilesFieldUpdating(PXCache cache, PXFieldUpdatingEventArgs args)
        {
            base.noteFilesFieldUpdating(cache, args);
            if (cache.Graph.IsMobile && args.NewValue is Guid[] fileIds && args.Row is Photo photo)
            {
                DeleteAttachedPhoto(cache, photo);
                var fileId = fileIds.First();
                var imageUrl = ComposeImageUrl(fileId);
                var file = GetFiles(cache.Graph, fileId).Single();
                FillPhotoFields(cache.Graph.Accessinfo, photo, imageUrl, fileId, file.Name);
                // Acuminator disable once PX1043 SavingChangesInEventHandlers [Justification]
                UpdatePhoto(cache, photo);
            }
        }

        private static void UpdatePhoto(PXCache cache, Photo photo)
        {
            cache.Update(photo);
            cache.PersistUpdated(photo);
            cache.Persisted(false);
            cache.Graph.SelectTimeStamp();
        }

        private static void FillPhotoFields(AccessInfo accessInfo, Photo photo, string imageUrl, Guid fileId,
            string fileName)
        {
            photo.ImageUrl = imageUrl;
            photo.FileId = fileId;
            photo.Name = fileName;
            photo.UploadedDate = accessInfo.BusinessDate;
            photo.UploadedById = accessInfo.UserID;
        }

        private static string ComposeImageUrl(Guid? fileId)
        {
            return string.Concat(PXUrl.SiteUrlWithPath(), Constants.FileUrl, fileId);
        }

        private static void DeleteAttachedPhoto(PXCache cache, Photo photo)
        {
            var fileNotes = GetFileNotes(cache, photo);
            if (fileNotes.HasAtLeastTwoItems())
            {
                var files = GetFiles(cache.Graph, fileNotes).ToList();
                var firstFile = files.GetItemWithMin(file => file.CreatedDateTime.GetValueOrDefault());
                UploadFileMaintenance.DeleteFile(firstFile.FileID);
            }
        }

        private static IEnumerable<UploadFile> GetFiles(PXGraph graph, params Guid[] fileIds)
        {
            return SelectFrom<UploadFile>
                .Where<UploadFile.fileID.IsIn<P.AsGuid>>.View.Select(graph, fileIds).FirstTableItems;
        }
    }
}