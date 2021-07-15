using Base64ConverterCore;
using GifToBase64Extractor.Modules.Models;
using MediaToBase64ExtractorCore.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GifToBase64Extractor.Modules
{
    class DragonRush_ImageExtractor : IMediaExtractor
    {
        List<string> animationTypes = new List<string>()
        {
            "a1",
            "a2",
            "bs",
            "op",
            "mo",
            "sw",
            "cha",
            "ult",
            "sup",
            "blo",
            "dmg",
        };
        Base64Converter base64Converter = new Base64Converter();

        string operationDirectory = Path.GetFullPath(@"operations");
        string saveDirectory = Path.GetFullPath(@"results");

        public void Operate()
        {
            if (!Directory.Exists(operationDirectory))
            {
                Console.WriteLine(operationDirectory);
                Console.WriteLine("creating operation Directory...");
                Directory.CreateDirectory(operationDirectory);
            }
            if (!Directory.Exists(saveDirectory))
            {
                Console.WriteLine(saveDirectory);
                Console.WriteLine("creating operation Directory...");
                Directory.CreateDirectory(saveDirectory);
            }

            #region get files
            List<string> files = getFilesFromOperationDirectory(operationDirectory + "/", "*.png");
            // unit media
            List<string> artworks = files.FindAll(x => Path.GetFileName(x).Split('_')[0] == "ART");
            List<string> avs = files.FindAll(x => Path.GetFileName(x).Split('_')[0] == "av");
            List<string> buttons = files.FindAll(x => Path.GetFileName(x).Split('_')[0] == "BTN");
            List<string> fazs = files.FindAll(x => Path.GetFileName(x).Split('_')[0] == "FAZ");
            // media
            List<string> bgs = files.FindAll(x => Path.GetFileName(x).StartsWith("BG"));
            List<string> banners = files.FindAll(x => Path.GetFileName(x).Split('_')[0] == "ban");
            // cards
            List<string> cardfiles = files.FindAll(x => (Path.GetFileName(x).StartsWith("B") && !bgs.Contains(x) && !buttons.Contains(x)) || Path.GetFileName(x).StartsWith("C"));
            // Custom
            List<string> artefacts = files.FindAll(x => Path.GetFileName(x).StartsWith("A") && !artworks.Contains(x) && !avs.Contains(x));
            List<string> equipments = files.FindAll(x => Path.GetFileName(x).StartsWith("E"));
            #endregion

            Console.WriteLine("following files are detedted...");
            foreach (string item in files)
                Console.WriteLine(item);

            Console.WriteLine("Reading OperationFolder for UnitMedia...");
            List<DragonRush_UnitMedia> unitMedia = GetMediaModelsV2(artworks, avs, buttons, fazs);

            Console.WriteLine("Reading OperationFolder for Equipment...");
            List<DragonRush_MediaModel> eqpList = GetCustomModelsV2(equipments, true);

            Console.WriteLine("Reading OperationFolder for Artefacts...");
            List<DragonRush_MediaModel> artList = GetCustomModelsV2(artefacts, false);

            Console.WriteLine("Reading OperationFolder for Banner...");
            List<DragonRush_MediaModel> bannerList = GetBannerV2(banners);

            Console.WriteLine("Reading OperationFolder for Battlegrounds...");
            List<DragonRush_MediaModel> bgList = GetBgsV2(bgs);

            Console.WriteLine("Reading OperationFolder for Cards...");
            List<DragonRush_MediaModel> cardList = GetCardsV2(cardfiles, false);

            Console.WriteLine("Reading OperationFolder for Basic Cards...");
            List<DragonRush_MediaModel> basicCardList = GetCardsV2(cardfiles, true);

            saveFileTask(unitMedia, eqpList, artList, bannerList, bgList, cardList, basicCardList);

            Console.WriteLine("operation complete!");
        }

        #region GetModels
        private List<DragonRush_UnitMedia> GetMediaModelsV2(List<string> artworks, List<string> avs, List<string> buttons, List<string> fazs)
        {
            List<DragonRush_UnitMedia> data = new List<DragonRush_UnitMedia>();

            #region anonymos fuctions
            List<DragonRush_UnitMedia> buildListV1(List<string> list, List<DragonRush_UnitMedia> itemList)
            {
                if (list.Count < 1) return new List<DragonRush_UnitMedia>();
                int mode = Path.GetFileName(list[0]).Split('_')[0] == "ART" ? 0 :
                    Path.GetFileName(list[0]).Split('_')[0] == "BTN" ? 1 :
                    Path.GetFileName(list[0]).Split('_')[0] == "av" ? 3 : 2;

                foreach (string item in list)
                {
                    int id = Convert.ToInt32(mode != 3 ? Path.GetFileName(item).Split('_')[1].Substring(1) : Path.GetFileName(item).Split('_')[1]);
                    int fid = Convert.ToInt32(mode != 3 ? Path.GetFileName(item).Split('_')[2].Split('.')[0].Substring(1) : Path.GetFileName(item).Split('_')[2].Split('.')[0]);
                    string base64 = base64Converter.ConvertToBase64(new Bitmap(item));

                    if (itemList.FindAll(x => x.id == id).Count > 0)
                    {
                        if (itemList.Where(x => x.id == id).FirstOrDefault()
                        .media.FindAll(y => y.fid == fid).Count > 0)
                        {
                            // adding to existent
                            if (mode == 0)
                                itemList.Where(x => x.id == id).FirstOrDefault().media.Where(y => y.fid == fid).FirstOrDefault().artwork = base64;
                            else if (mode == 1)
                                itemList.Where(x => x.id == id).FirstOrDefault().media.Where(y => y.fid == fid).FirstOrDefault().button = base64;
                            else if (mode == 2)
                                itemList.Where(x => x.id == id).FirstOrDefault().media.Where(y => y.fid == fid).FirstOrDefault().faz = base64;
                            else if (mode == 3)
                                itemList.Where(x => x.id == id).FirstOrDefault().media.Where(y => y.fid == fid).FirstOrDefault().av = base64;
                        }
                        else
                        {
                            // add new item to data.media
                            if (mode == 0)
                                data.Where(x => x.id == id).FirstOrDefault().media.Add(new DragonRush_UnitMediaModel()
                                {
                                    id = id,
                                    fid = fid,
                                    artwork = base64
                                });
                            else if (mode == 1)
                                data.Where(x => x.id == id).FirstOrDefault().media.Add(new DragonRush_UnitMediaModel()
                                {
                                    id = id,
                                    fid = fid,
                                    button = base64
                                });
                            else if (mode == 2)
                                data.Where(x => x.id == id).FirstOrDefault().media.Add(new DragonRush_UnitMediaModel()
                                {
                                    id = id,
                                    fid = fid,
                                    faz = base64
                                });
                            else if (mode == 3)
                                data.Where(x => x.id == id).FirstOrDefault().media.Add(new DragonRush_UnitMediaModel()
                                {
                                    id = id,
                                    fid = fid,
                                    av = base64
                                });
                        }
                    }
                    else
                    {
                        DragonRush_UnitMediaModel model = new DragonRush_UnitMediaModel();
                        model.id = id;
                        model.fid = fid;
                        if (mode == 0)
                            model.artwork = base64;
                        else if (mode == 1)
                            model.button = base64;
                        else if (mode == 2)
                            model.faz = base64;
                        else if (mode == 3)
                            model.av = base64;

                        // add new
                        data.Add(new DragonRush_UnitMedia()
                        {
                            id = id,
                            media = new List<DragonRush_UnitMediaModel>()
                            {
                                model
                            }
                        });
                    }
                }

                return data;
            }
            #endregion

            data = buildListV1(artworks, data);
            data = buildListV1(buttons, data);
            data = buildListV1(fazs, data);
            data = buildListV1(avs, data);

            return data;
        }

        private List<DragonRush_MediaModel> GetCustomModelsV2(List<string> items, bool isEqupment)
        {
            List<DragonRush_MediaModel> customs = new List<DragonRush_MediaModel>();

            foreach (string item in items)
            {
                int id = Convert.ToInt32(Path.GetFileName(item).Substring(1).Split('.')[0]);
                string base64 = base64Converter.ConvertToBase64(new Bitmap(item));
                customs.Add(new DragonRush_MediaModel()
                {
                    id = id,
                    filename = Path.GetFileName(item).Split('.')[0],
                    type = isEqupment ? "eqp" : "art",
                    base64String = base64
                });
            }
            return customs;
        }

        private List<DragonRush_MediaModel> GetBgsV2(List<string> bgs)
        {
            List<DragonRush_MediaModel> data = new List<DragonRush_MediaModel>();
            foreach (string item in bgs)
            {
                DragonRush_MediaModel model = new DragonRush_MediaModel()
                {
                    type = "bg",
                    filename = Path.GetFileName(item).Split('.')[0],
                    base64String = base64Converter.ConvertToBase64(new Bitmap(item))
                };
                data.Add(model);
            }

            return data;
        }

        private List<DragonRush_MediaModel> GetBannerV2(List<string> banners)
        {
            List<DragonRush_MediaModel> data = new List<DragonRush_MediaModel>();

            foreach (string item in banners)
            {
                DragonRush_MediaModel model = new DragonRush_MediaModel()
                {
                    type = "banner",
                    filename = Path.GetFileName(item).Split('.')[0],
                    id = Convert.ToInt32(Path.GetFileName(item).Split('.')[0].Split('_')[1]),
                    base64String = base64Converter.ConvertToBase64(new Bitmap(item))
                };
                data.Add(model);
            }

            return data;
        }

        private List<DragonRush_MediaModel> GetCardsV2(List<string> cards, bool returnBasic)
        {
            List<DragonRush_MediaModel> data = new List<DragonRush_MediaModel>();

            foreach (string item in cards)
            {
                if ((Path.GetFileName(item).StartsWith("B") && returnBasic) ||
                (!returnBasic && Path.GetFileName(item).StartsWith("C")))
                {
                    DragonRush_MediaModel model = new DragonRush_MediaModel()
                    {
                        type = "card",
                        filename = Path.GetFileName(item).Split('.')[0],
                        id = Convert.ToInt32(Path.GetFileName(item).Split('.')[0].Substring(1)),
                        base64String = base64Converter.ConvertToBase64(new Bitmap(item))
                    };

                    data.Add(model);
                }
            }

            return data;
        }
        #endregion

        #region Saving Tasks
        private async Task SaveUnitMedias(List<DragonRush_UnitMedia> unitMedia)
        {
            List<Task> activeTasks = new List<Task>();

            #region unitMedia
            Console.WriteLine("saving UnitMedia...");
            foreach (DragonRush_UnitMedia item in unitMedia)
            {
                string Prefix = item.id > 9 ? item.id.ToString() : '0' + item.id.ToString();
                Task task = Task.Run(() => taskRunnerAsync(item, $"{Prefix}-unitMedia"));
                activeTasks.Add(task);
            }
            await Task.WhenAll(activeTasks.ToArray());
            Console.WriteLine("UnitMedia Complete!");
            activeTasks.Clear();
            #endregion
        }
        private async Task SaveEquipments(List<DragonRush_MediaModel> eqpList)
        {
            List<Task> activeTasks = new List<Task>();

            #region equipment
            Console.WriteLine("saving Equipment...");
            foreach (DragonRush_MediaModel item in eqpList)
            {
                string Prefix = item.id > 9 ? item.id.ToString() : '0' + item.id.ToString();
                Task task = Task.Run(() => taskRunnerAsync(item, $"{Prefix}-equipment"));
                activeTasks.Add(task);
            }
            await Task.WhenAll(activeTasks.ToArray());
            Console.WriteLine("Equipment Complete!");
            activeTasks.Clear();
            #endregion
        }
        private async Task SaveArtefacts(List<DragonRush_MediaModel> artList)
        {
            List<Task> activeTasks = new List<Task>();
            Console.WriteLine("saving Artefacts...");

            foreach (DragonRush_MediaModel item in artList)
            {
                string Prefix = item.id > 9 ? item.id.ToString() : '0' + item.id.ToString();
                Task task = Task.Run(() => taskRunnerAsync(item, $"{Prefix}-artefact"));
                activeTasks.Add(task);
            }
            await Task.WhenAll(activeTasks.ToArray());

            Console.WriteLine("Artefacts Complete!");
        }
        private async Task SaveCards(List<DragonRush_MediaModel> cardList)
        {
            List<Task> activeTasks = new List<Task>();
            Console.WriteLine("saving Cards...");

            foreach (DragonRush_MediaModel item in cardList)
            {
                string prefix = item.id > 9 ? item.id.ToString() : '0' + item.id.ToString();
                Task task = Task.Run(() => taskRunnerAsync(item, $"{prefix}-card"));
                activeTasks.Add(task);
            }
            await Task.WhenAll(activeTasks.ToArray());

            Console.WriteLine("Cards Complete!");
        }
        private async Task SaveBasicCard(List<DragonRush_MediaModel> basicCardList)
        {
            List<Task> activeTasks = new List<Task>();
            #region cards Basic
            Console.WriteLine("saving Basic Cards...");
            foreach (DragonRush_MediaModel item in basicCardList)
            {
                Task task = Task.Run(() => taskRunnerAsync(item, $"{item.id}-Bcard"));
                activeTasks.Add(task);
            }
            await Task.WhenAll(activeTasks.ToArray());
            Console.WriteLine("Basic Cards Complete!");
            activeTasks.Clear();
            #endregion
        }
        private async Task SaveBattlegrounds(List<DragonRush_MediaModel> bgList)
        {
            List<Task> activeTasks = new List<Task>();
            #region bgs
            Console.WriteLine("saving Bgs...");
            foreach (DragonRush_MediaModel item in bgList)
            {
                string prefix = item.id > 9 ? item.id.ToString() : '0' + item.id.ToString();
                Task task = Task.Run(() => taskRunnerAsync(item, $"{prefix}-bg"));
                activeTasks.Add(task);
            }
            await Task.WhenAll(activeTasks.ToArray());
            Console.WriteLine("Bgs Complete!");
            activeTasks.Clear();
            #endregion
        }
        private async Task SaveBanners(List<DragonRush_MediaModel> bannerList)
        {
            List<Task> activeTasks = new List<Task>();
            #region banner
            Console.WriteLine("saving Banner...");
            foreach (DragonRush_MediaModel item in bannerList)
            {
                string prefix = item.id > 9 ? item.id.ToString() : '0' + item.id.ToString();
                Task task = Task.Run(() => taskRunnerAsync(item, $"{prefix}-banner"));
                activeTasks.Add(task);
            }
            await Task.WhenAll(activeTasks.ToArray());
            Console.WriteLine("Banner Complete!");
            activeTasks.Clear();
            #endregion
        }
        #endregion

        private async void saveFileTask(List<DragonRush_UnitMedia> unitMedia, 
        List<DragonRush_MediaModel> eqpList, List<DragonRush_MediaModel> artList,
        List<DragonRush_MediaModel> bannerList, List<DragonRush_MediaModel> bgList, 
        List<DragonRush_MediaModel> cardList, List<DragonRush_MediaModel> basicCardList)
        {
            Console.WriteLine("saving Files...");

            await SaveUnitMedias(unitMedia);
            await SaveEquipments(eqpList);
            await SaveArtefacts(artList);
            await SaveBanners(bannerList);
            await SaveBattlegrounds(bgList);
            await SaveCards(cardList);
            await SaveBasicCard(basicCardList);
        }

        private static List<string> getFilesFromOperationDirectory(string path, string extention)
        {
            if (Directory.Exists(path))
            {
                return Directory.GetFiles(path).ToList();
            }
            else
                return new List<string>();
        }

        private async Task taskRunnerAsync(DragonRush_MediaModel obj, string filename)
        {
            await SaveAsJasonAsync(obj, filename);
        }
        private async Task taskRunnerAsync(DragonRush_UnitMedia obj, string filename)
        {
            await SaveAsJasonAsync(obj, filename);
        }

        #region save
        //private void SaveAsJason(DragonRush_UnitMedia data, string filename)
        //{
        //    string json = JsonSerializer.Serialize(data);
        //    File.WriteAllText(@"results/" + filename + ".json", json);
        //}
        private async Task SaveAsJasonAsync(DragonRush_MediaModel obj, string filename)
        {
            // maybe formated
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(saveDirectory + "/" + filename + ".json", json);
        }
        private async Task SaveAsJasonAsync(DragonRush_UnitMedia obj, string filename)
        {
            // maybe formated
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(saveDirectory + "/" + filename + ".json", json);
        }
        #endregion
    }
}
