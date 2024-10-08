using WatersAD.Data.Entities;
using WatersAD.Enum;
using WatersAD.Models;

namespace WatersAD.Helpers
{
    public interface IConverterHelper
    {
        /// <summary>
        /// Converts a RegisterNewUserViewModel to a User object
        /// </summary>
        /// <param name="model">The view model containing the data</param>
        /// <param name="path">Path for profile picture</param>
        /// <returns>User</returns>
        User ToUser(RegisterNewUserViewModel model, string path);


        /// <summary>
        ///  Converts a ChangeUserViewModel to a User object
        /// </summary>
        /// <param name="model">The view model containing the data</param>
        /// <param name="path"></param>
        /// <returns>User</returns>
        User ToUser(ChangeUserViewModel model, string path);

        /// <summary>
        /// Converts a role name into a corresponding enumeration value.
        /// </summary>
        /// <param name="roleName">The name of the role to be converted</param>
        /// <returns>A value that corresponds to the specified role name.</returns>
        UserType ConvertRoleToUserType(string roleName);

        /// <summary>
        /// Converts a <see cref="ClientViewModel"/> object to a <see cref="Client"/> object.
        /// </summary>
        /// <param name="client">The <see cref="ClientViewModel"/> object to be converted.</param>
        /// <returns>A <see cref="Client"/> object that represents the converted data from the <see cref="ClientViewModel"/>.</returns>
        Client ToCliente(ClientViewModel client, Locality locality);

        /// <summary>
        /// Converts a <see cref="Client"/> object to a <see cref="ClientViewModel"/> object.
        /// </summary>
        /// <param name="client">The <see cref="Client"/> object to be converted.</param>
        /// <returns>A task that represents the operation. The task result contains a <see cref="ClientViewModel"/> object that represents the converted data from the <see cref="Client"/>.</returns>
        ClientViewModel ToClientViewModel(Client client);


        /// <summary>
        /// Converts an <see cref="EmployeeViewModel"/> into an <see cref="Employee"/> object, associating it with a specific locality.
        /// </summary>
        /// <param name="employee">The <see cref="EmployeeViewModel"/> object containing employee data.</param>
        /// <param name="locality">The <see cref="Locality"/> where the employee is located or assigned.</param>
        /// <returns>An <see cref="Employee"/> object created from the given employee model and locality.</returns>
        Employee ToEmployee(EmployeeViewModel employee, Locality locality);

        /// <summary>
        /// Converts an <see cref="Employee"/> object into an <see cref="EmployeeViewModel"/> object for use in view models or data transfer operations.
        /// </summary>
        /// <param name="employee">The <see cref="Employee"/> object containing the employee's data.</param>
        /// <returns>An <see cref="EmployeeViewModel"/> object created from the employee data.</returns>
        EmployeeViewModel ToEmployeeViewModel(Employee employee);
    }
}
