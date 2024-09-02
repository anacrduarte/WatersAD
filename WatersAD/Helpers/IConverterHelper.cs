using WatersAD.Data.Entities;
using WatersAD.Models;

namespace WatersAD.Helpers
{
    public interface IConverterHelper
    {
        Client ToClient(ClientViewModel model, string path, bool isNew);

        ClientViewModel ToClientViewModel(Client client);
    }
}
