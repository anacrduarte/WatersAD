using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Net;
using WatersAD.Data.Entities;
using WatersAD.Data.Repository;
using WatersAD.Enum;
using WatersAD.Models;

namespace WatersAD.Helpers
{
    public class ConverterHelper : IConverterHelper
    {
        private readonly ICountryRepository _countryRepository;

        public ConverterHelper(ICountryRepository countryRepository)
        {
            _countryRepository = countryRepository;
        }
        public RegisterNewUserViewModel ToRegisterNewUserViewModel(User user)
        {
            throw new NotImplementedException();
        }

        public User ToUser(RegisterNewUserViewModel model, string path)
        {
            UserType userType = ConvertRoleToUserType(model.SelectedRole);

            return new User
            {
                
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Username,
                UserName = model.Username,
                Address = model.Address,
                ImageUrl = path,
                UserType = userType,

            };
        }

        public User ToUser(ChangeUserViewModel model, string path)
        {
            return new User
            {

                FirstName = model.FirstName,
                LastName = model.LastName,
                ImageUrl = path,

            };
        }

        public UserType ConvertRoleToUserType(string roleName)
        {
            switch (roleName)
            {
                case "Admin":
                    return UserType.Admin;
                case "Customer":
                    return UserType.Customer;
                case "Employee":
                    return UserType.Employee;
                default:
                    throw new ArgumentException($"Role inválida: {roleName}");
            }
        }

        public Client ToCliente(ClientViewModel client, Locality locality)
        {
            return new Client
            {
                FirstName = client.FirstName,
                LastName = client.LastName,
                Address = client.Address,
                Email = client.Email,
                LocalityId = locality.Id,
                PhoneNumber = client.PhoneNumber,
                PostalCode = client.PostalCode,
                RemainPostalCode = client.RemainPostalCode,
                HouseNumber = client.HouseNumber,
                NIF = client.NIF,
                User = client.User,
                WaterMeters = new List<WaterMeter>(),
                
            };
        }

        public WaterMeter ToWaterMeter(ClientViewModel model, int clientId, Locality locality, WaterMeterService service)
        {
            return new WaterMeter
            {
                ClientId = clientId,
                Address = model.Address,
                HouseNumber = model.HouseNumber,
                InstallationDate = model.InstallationDate,
                LocalityId = locality.Id,
                WaterMeterServiceId = service.Id ,
                IsActive = true,
            };
        }

        public ClientViewModel ToClientViewModel(Client client)
        {

            var model = new ClientViewModel
            {
                Client = client,
                FirstName = client.FirstName,
                LastName = client.LastName,
                PhoneNumber = client.PhoneNumber,
                ClientId = client.Id,
                Address = client.Address,
                HouseNumber = client.HouseNumber,
                PostalCode = client.PostalCode,
                RemainPostalCode = client.RemainPostalCode,
                LocalityId = client.LocalityId,
                Locality = client.Locality,
                CountryId = client.Locality.City.CountryId,
                Country = client.Locality.City.Country,
                CityId = client.Locality.CityId,
                City = client.Locality.City,
                Email = client.Email,

            };
        

            return model;
        }
    }
}
