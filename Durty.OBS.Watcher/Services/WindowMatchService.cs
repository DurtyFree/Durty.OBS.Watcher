namespace Durty.OBS.Watcher.Services
{
    public class WindowMatchService
    {
        public bool DoesTitleMatch(string fullTitle, string titleSearch)
        {
            //TODO: Implement super crazy search / contains / placeholder/ variables thing
            //"Durtys OBS Watcher - Microsoft Visual Studio" Matches with "Durtys%Watcher%Microsoft Visual Studio"
            return fullTitle.Contains(titleSearch);
        }
    }
}
