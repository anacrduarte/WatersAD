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

        public Client ToCliente(ClientViewModel client)
        {
            return new Client
            {
                FirstName = client.FirstName,
                LastName = client.LastName,
                Address = client.Address,
                Email = client.Email,
                LocalityId = client.LocalityId,
                PhoneNumber = client.PhoneNumber,
                NIF = client.NIF,
                User = client.User,
            };
        }

        public async Task<ClientViewModel> ToClientViewModel(Client client)
        {
            var model = new ClientViewModel();

            model.ClientId = client.Id;
            model.FirstName = client.FirstName;
            model.LastName = client.LastName;
            model.Address = client.Address;
            model.Email = client.Email;
            model.PhoneNumber = client.PhoneNumber;
            model.NIF = client.NIF;
            model.User = client.User;

            try
            {

                var locality = await _countryRepository.GetLocalityAsync(client.LocalityId);

                if (locality != null)
                {
                    var city = await _countryRepository.GetCityAsync(locality.CityId);

                    if (city != null)
                    {
                        var country = await _countryRepository.GetCountryAsync(city);

                        if (country != null)
                        {
                            model.CountryId = country.Id; ;
                            model.Cities = _countryRepository.GetComboCities(country.Id);
                            model.Countries = _countryRepository.GetComboCountries();
                            model.CityId = city.Id;
                            model.Localities = _countryRepository.GetComboLocalities(city.Id);
                            model.LocalityId = locality.Id;


                        }
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }

            return model;
        }
    }
}
