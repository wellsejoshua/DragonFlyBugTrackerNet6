﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DragonFlyBugTrackerNet6.Models;

namespace DragonFlyBugTrackerNet6.Services.Interfaces
{
    public interface IBTRolesService
    {
        public Task<bool> IsUserInRoleAsync(AppUser user, string roleName);

        public Task<List<IdentityRole>> GetRolesAsync();

        public Task<IEnumerable<string>> GetUserRolesAsync(AppUser user);

        public Task<bool> AddUserToRoleAsync(AppUser user, string roleName);

        public Task<bool> AddUserToRolesAsync(AppUser user, IEnumerable<string> roles);

        public Task<bool> RemoveUserFromRoleAsync(AppUser user, string roleName);

        public Task<bool> RemoveUserFromRolesAsync(AppUser user, IEnumerable<string> roles);

        public Task<List<AppUser>> GetUsersInRoleAsync(string roleName, int companyId);

        public Task<List<AppUser>> GetUsersNotInRoleAsync(string roleName, int companyId);

        public Task<string> GetRoleNameByIdAsync(string roleId);
    }
}
