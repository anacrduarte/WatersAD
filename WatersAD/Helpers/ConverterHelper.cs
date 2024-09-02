using WatersAD.Data.Entities;
using WatersAD.Models;

namespace WatersAD.Helpers
{
    public class ConverterHelper : IConverterHelper
    {
        public Client ToClient(ClientViewModel model, string path, bool isNew)
        {
            return new Client
            {
                Id = isNew ? 0 : model.Id,
                ImageUrl = path,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Address = model.Address,
                NIF = model.NIF,
                PhoneNumber = model.PhoneNumber,
                User = model.User,

            };
        }

        public ClientViewModel ToClientViewModel(Client client)
        {
            return new ClientViewModel
            {
                Id = client.Id,
                ImageUrl = client.ImageUrl,
                FirstName = client.FirstName,
                LastName = client.LastName,
                Address = client.Address,
                NIF = client.NIF,
                PhoneNumber = client.PhoneNumber,
                User = client.User,

            };
        }
    }
}
