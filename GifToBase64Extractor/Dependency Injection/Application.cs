using MediaToBase64ExtractorCore.Utilities;

namespace GifToBase64Extractor.Dependency_Injection
{
    public class Application : IApplication
    {
        IMediaExtractor extractor;
        public Application(IMediaExtractor programm)
        {
            extractor = programm;
        }

        public void Run()
        {
            extractor.Operate();
        }
    }
}
