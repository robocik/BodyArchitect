using System;
using System.Collections.Generic;
using System.IO;
using BodyArchitect.Logger;
using BodyArchitect.Model;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Shared;
using NHibernate;

namespace BodyArchitect.Service.V2.Services
{
    public class PictureService:ServiceBase
    {

        public PictureService(ISession session, SecurityInfo securityInfo, ServiceConfiguration configuration)
            : base(session, securityInfo, configuration)
        {

        }

        public void DeletePictureLogic(Picture oldPicture,PictureInfoDTO newPicture)
        {
            if (oldPicture != null && (newPicture == null || (newPicture.Hash != oldPicture.Hash && newPicture.PictureId != oldPicture.PictureId)))
            {
                PictureService pictureService = new PictureService(Session, SecurityInfo, Configuration);
                pictureService.DeletePicture(oldPicture);
            }
        }

        public void DeletePicture(Picture picture)
        {
            Log.WriteInfo("Removing picture...");
            // delete target file, if already exists
            try
            {
                string filePath = System.IO.Path.Combine(Configuration.ImagesFolder, picture.PictureId.ToString());
                if (System.IO.File.Exists(filePath))
                {
                    Log.WriteVerbose("File exists: {0}", filePath);
                    System.IO.File.Delete(filePath);
                    Log.WriteVerbose("File deleted");
                }
                else
                {
                    Log.WriteVerbose("File not found: {0}", filePath);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.Default.Process(ex);
            }
        }

        public PictureInfoDTO UploadImage(PictureDTO pictureDto)
        {
            Log.WriteWarning("UploadImage:Username={0},pictureId: {1}", SecurityInfo.SessionData.Profile.UserName, pictureDto.PictureId);
            var pictureId = pictureDto.PictureId != Guid.Empty ? pictureDto.PictureId : Guid.NewGuid();
            // create output folder, if does not exist)
            if (!System.IO.Directory.Exists(Configuration.ImagesFolder))
            {
                System.IO.Directory.CreateDirectory(Configuration.ImagesFolder);
            }

            // delete target file, if already exists
            string filePath = System.IO.Path.Combine(Configuration.ImagesFolder, pictureId.ToString());
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            MemoryStream memoryStream = new MemoryStream();
            pictureDto.ImageStream.CopyTo(memoryStream);

            using (System.IO.FileStream writeStream = new System.IO.FileStream(filePath, System.IO.FileMode.CreateNew, System.IO.FileAccess.ReadWrite))
            {
                memoryStream.Seek(0, SeekOrigin.Begin);
                memoryStream.CopyTo(writeStream);
                writeStream.Close();
            }
            memoryStream.Seek(0, SeekOrigin.Begin);
            string hash = CryptographyHelper.GetMD5HashFromFile(memoryStream);

            var pictureInfo = new PictureInfoDTO(pictureId, hash);
            return pictureInfo;
        }

        public PictureDTO GetImage(PictureInfoDTO pictureInfo)
        {
            
            Log.WriteWarning("GetImage:Username={0},PictureId:{1}", SecurityInfo.SessionData.Profile.UserName, pictureInfo.PictureId);

            string filePath = System.IO.Path.Combine(Configuration.ImagesFolder, pictureInfo.PictureId.ToString());
            if (!System.IO.File.Exists(filePath))
            {
                throw new FileNotFoundException("Image file not found");
            }
            PictureDTO dto = new PictureDTO(pictureInfo.PictureId, pictureInfo.Hash);
            MemoryStream memoryStream = new MemoryStream();
            using (var fileStream = File.OpenRead(filePath))
            {
                fileStream.CopyTo(memoryStream);
            }
            memoryStream.Seek(0, SeekOrigin.Begin);
            dto.ImageStream = memoryStream;
            Log.WriteInfo("Picture retrieved");
            return dto;
        }
    }
}
