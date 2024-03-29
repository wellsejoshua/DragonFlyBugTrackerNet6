﻿using DragonFlyBugTrackerNet6.Models.ViewModels;
using DragonFlyBugTrackerNet6.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DragonFlyBugTrackerNet6.Extensions;
using DragonFlyBugTrackerNet6.Models;
using DragonFlyBugTrackerNet6.Models.ViewModels;
using DragonFlyBugTrackerNet6.Services.Interfaces;

namespace TheBugTracker.Controllers
{
    [Authorize]
    public class UserRolesController : Controller
    {
        private readonly IBTRolesService _rolesService;
        private readonly IBTCompanyInfoService _companyInfoService;

        public UserRolesController(IBTRolesService rolesService, IBTCompanyInfoService companyInfoService)
        {
            _rolesService = rolesService;
            _companyInfoService = companyInfoService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ManageUserRoles()
        {
            //add instance of the ViewModel as a List(model)
            List<ManageUserRolesViewModel> model = new();

            //Get CompanyId
            int companyId = User.Identity.GetCompanyId().Value;

            //Get all company users
            List<AppUser> usersNonDemo = await _rolesService.GetUsersNotInRoleAsync("DemoUser", companyId);
            
           

            //Loop over the users to populate the ViewModel
            foreach (AppUser user in usersNonDemo)
            {
                ManageUserRolesViewModel viewModel = new();
                viewModel.AppUser = user;
                IEnumerable<string> selected = await _rolesService.GetUserRolesAsync(user);
                viewModel.Roles = new MultiSelectList(await _rolesService.GetRolesAsync(), "Name", "Name", selected);

                model.Add(viewModel);
            }
            //  -  instantiate the ViewModel
            //  -  use rolesService
            //Create multi-selects
            return View(model);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageUserRoles(ManageUserRolesViewModel member)
        {

            //Get the company Id
            int companyId = User.Identity.GetCompanyId().Value;

            //Instantiate the BTUser
            AppUser btUser = (await _companyInfoService.GetAllMembersAsync(companyId)).FirstOrDefault(u => u.Id == member.AppUser.Id);
            //Get Roles for the User
            IEnumerable<string> roles = await _rolesService.GetUserRolesAsync(btUser);
            //grab the selected Role
            string userRole = member.SelectedRoles.FirstOrDefault();

            //Get Roles from the select list
            IEnumerable<string> rolesSelectList = member.SelectedRoles;


            if (!string.IsNullOrEmpty(userRole))
            {
                //Remove User from their roles (only if user actually selected a role)
                if (await _rolesService.RemoveUserFromRolesAsync(btUser, roles))
                {
                    //Add User to the new role
                    //await _rolesService.AddUserToRoleAsync(btUser, userRole);
                    await _rolesService.AddUserToRolesAsync(btUser, rolesSelectList);
                }
            }

            //Navigate back to view
            return RedirectToAction(nameof(ManageUserRoles));




        }
    }
}

