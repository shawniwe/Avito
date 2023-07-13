using System.Buffers.Text;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using Avito2.Abstract;
using Avito2.Domains;
using Avito2.Models;
using Microsoft.EntityFrameworkCore;
using static System.Net.Mime.MediaTypeNames;

namespace Avito2.BlazorServices
{
    public class AdvertisementService
    {
        private readonly IRepository<Advertisement> advertisementRepository;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<Photo> _photoRepository;
        private readonly IRepository<AdvertisementStatement> _statementRepository;

        public AdvertisementService(IRepository<Advertisement> advertisementRepository,
            IRepository<Category> categoryRepository,
            IRepository<Photo> photoRepository,
            IRepository<AdvertisementStatement> statementRepository)
        {
            this.advertisementRepository = advertisementRepository;
            _categoryRepository = categoryRepository;
            _photoRepository = photoRepository;
            _statementRepository = statementRepository;
        }

        public IEnumerable<Advertisement> GetAdvertisements(Func<Advertisement, bool> predicate)
        {
            return advertisementRepository.ReadList().Where(predicate);
        }
        public IEnumerable<Advertisement> GetAdvertisements()
        {
            return advertisementRepository.ReadList();
        }


        private List<string> UploadImages(List<string> strPhotos)
        {
            if (strPhotos == null)
            {
                Debug.WriteLine("картинка не найдена");
                return null;
            }

            List<string> loadedFiles = new List<string>();

            foreach (var str in strPhotos.Where(x => x.Contains("base64")))
            {
                var regExt = Regex.Matches(str.Substring(0, 50), @"(?<=data:image\/)png(?=;)")[0];
                string ext = "." + regExt;
                string uniqueName = Guid.NewGuid().ToString() + ext;
                string filename = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    @"wwwroot\images",
                    uniqueName);
                using (var stream = System.IO.File.Create(filename))
                {
                    stream.Write(Convert.FromBase64String(str.Split(',')[1]));
                }

                loadedFiles.Add(uniqueName);
            }
            return loadedFiles;
        }

        public long? CreateAdvertisement(AdEditModel model, List<string> strPhotos, string userId)
        {
            try
            {
                var files = UploadImages(strPhotos);

                var category = _categoryRepository
                    .ReadList()
                    .FirstOrDefault(x => x.Id == model.Category);

                var moderatorStatement = _statementRepository.ReadList().FirstOrDefault(x => x.Name == "На модерации");

                var ad = new Advertisement()
                {
                    Category = category,
                    Title = model.Title,
                    Address = model.Address,
                    City = model.City,
                    Description = model.Description,
                    PlacementDate = DateTime.Now,
                    Price = model.Price,
                    Statement = moderatorStatement,
                    CreationAuthorId = userId
                };
                advertisementRepository.Create(ad);

                foreach (var f in files)
                {
                    var ph = new Photo()
                    {
                        FilePath = f,
                        Advertisement = ad

                    };
                    _photoRepository.Create(ph);
                }

                return ad.Id;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public long? UpdateAdvertisement(AdEditModel adModel, List<string> imagesList)
        {
            try
            {
                var files = UploadImages(imagesList);
                var entry = advertisementRepository.ReadList().FirstOrDefault(x => x.Id == adModel.Id);
                var category = _categoryRepository
                    .ReadList()
                    .FirstOrDefault(x => x.Id == adModel.Category);
                var moderatorStatement = _statementRepository.ReadList().FirstOrDefault(x => x.Name == "На модерации");
                entry.Category = category;
                entry.Address = adModel.Address;
                entry.City = adModel.City;
                entry.Description = adModel.Description;
                entry.PlacementDate = DateTime.Now;
                entry.Price = adModel.Price;
                entry.Statement = moderatorStatement;
                advertisementRepository.Update(entry);

                foreach (var file in files)
                {
                    var ph = new Photo()
                    {
                        Advertisement = entry,
                        FilePath = file
                    };

                    _photoRepository.Create(ph);
                }

                return entry.Id;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public void DeletePhoto(string image)
        {
            var entry = _photoRepository.ReadList().FirstOrDefault(x => x.FilePath == image);
            _photoRepository.Delete(entry);
        }
    }
}
