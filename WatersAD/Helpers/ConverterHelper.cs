﻿using WatersAD.Data.Entities;
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
                Invoices = new List<Invoice>(),


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
                NIF = client.NIF,
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
                User = client.User,

            };


            return model;
        }

        public Employee ToEmployee(EmployeeViewModel employee, Locality locality)
        {
            return new Employee
            {
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Address = employee.Address,
                Email = employee.Email,
                LocalityId = locality.Id,
                PhoneNumber = employee.PhoneNumber,
                PostalCode = employee.PostalCode,
                RemainPostalCode = employee.RemainPostalCode,
                HouseNumber = employee.HouseNumber,
                NIF = employee.NIF,
                User = employee.User,


            };
        }

        public EmployeeViewModel ToEmployeeViewModel(Employee employee)
        {

            var model = new EmployeeViewModel
            {
                EmployeeId = employee.Id,
                Employee = employee,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                PhoneNumber = employee.PhoneNumber,
                NIF = employee.NIF,
                Address = employee.Address,
                HouseNumber = employee.HouseNumber,
                PostalCode = employee.PostalCode,
                RemainPostalCode = employee.RemainPostalCode,
                LocalityId = employee.LocalityId,
                Locality = employee.Locality,
                CountryId = employee.Locality.City.CountryId,
                Country = employee.Locality.City.Country,
                CityId = employee.Locality.CityId,
                City = employee.Locality.City,
                Email = employee.Email,
                User = employee.User,
            };


            return model;
        }
    }
}
