﻿using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DragonFlyBugTrackerNet6.Data;
using DragonFlyBugTrackerNet6.Models;
using DragonFlyBugTrackerNet6.Models.Enums;
using DragonFlyBugTrackerNet6.Services.Interfaces;


namespace DragonFlyBugTrackerNet6.Services
{
  public class BTProjectService : IBTProjectService
  {
    private readonly ApplicationDbContext _context;
    private readonly IBTRolesService _rolesService;

    public BTProjectService(ApplicationDbContext context, IBTRolesService rolesService)
    {
      _context = context;
      _rolesService = rolesService;
    }

    //CRUD Create
    #region Add New Project
    public async Task AddNewProjectAsync(Project project)
    {
      _context.Add(project);
      await _context.SaveChangesAsync();

    }
    #endregion

    #region Add Project Manager Async
    public async Task<bool> AddProjectManagerAsync(string userId, int projectId)
    {
      
      AppUser currentPM = await GetProjectManagerAsync(projectId);
      AppUser newPM = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
      Project project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == projectId);

      //Remove Current PM if necessary
      if (currentPM != null)
      {
        try
        {
          await RemoveProjectManagerAsync(projectId);
        }
        catch (Exception ex)
        {

          Console.WriteLine($"Error removing current PM. - Error: {ex.Message}");
          return false;
        }
      }
      //Add new PM
      try
      {
        await AddUserToProjectAsync(userId, projectId);
        //await AddProjectManagerAsync(userId, projectId);
        return true;
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Error Adding PM. - Error: {ex.Message}");
        return false;
      }

    }

    #endregion

    #region Add User To Project
    public async Task<bool> AddUserToProjectAsync(string userId, int projectId)
    {
      AppUser user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

      if (user != null)
      {
        Project project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == projectId);
        if (!await IsUserOnProjectAsync(userId, projectId))
        {
          try
          {
            project.Members.Add(user);
            await _context.SaveChangesAsync();
            return true;
          }
          catch (Exception)
          {
            throw;
          }
        }
        else
        {
          return false;
        }
      }
      else
      {
        return false;
      }

    }

    #endregion

    // CRUD - Archive (Delete)

    #region Archive Project
    public async Task ArchiveProjectAsync(Project project)
    {
      try
      {
        project.Archived = true;
        await UpdateProjectAsync(project);

        //Archive the Tickets for the Project
        foreach (Ticket ticket in project.Tickets)
        {
          ticket.ArchivedByProject = true;
          _context.Update(ticket);
          await _context.SaveChangesAsync();
        }

      }
      catch (Exception)
      {

        throw;
      }

    }

    #endregion

    #region Get All Project Members Except Project Manager
    public async Task<List<AppUser>> GetAllProjectMembersExceptPMAsync(int projectId)
    {
      List<AppUser> developers = await GetProjectMembersByRoleAsync(projectId, Roles.Developer.ToString());
      List<AppUser> submitters = await GetProjectMembersByRoleAsync(projectId, Roles.Submitter.ToString());
      List<AppUser> admins = await GetProjectMembersByRoleAsync(projectId, Roles.Admin.ToString());

      List<AppUser> teamMembers = developers.Concat(submitters).Concat(admins).ToList();

      return teamMembers;
    }

    #endregion

    #region Get All Projects By Company
    public async Task<List<Project>> GetAllProjectsByCompanyAsync(int companyId)
    {
      List<Project> projects = new();
      projects = await _context.Projects.Where(p => p.CompanyId == companyId && p.Archived == false)
                                      .Include(p => p.Members)
                                      .Include(p => p.Tickets)
                                          .ThenInclude(t => t.Comments)
                                      .Include(p => p.Tickets)
                                          .ThenInclude(t => t.Attachments)
                                      .Include(p => p.Tickets)
                                          .ThenInclude(t => t.History)
                                      .Include(p => p.Tickets)
                                          .ThenInclude(t => t.DeveloperUser)
                                      .Include(p => p.Tickets)
                                          .ThenInclude(t => t.Notifications)
                                      .Include(p => p.Tickets)
                                          .ThenInclude(t => t.OwnerUser)
                                      .Include(p => p.Tickets)
                                          .ThenInclude(t => t.TicketStatus)
                                      .Include(p => p.Tickets)
                                          .ThenInclude(t => t.TicketPriority)
                                      .Include(p => p.Tickets)
                                          .ThenInclude(t => t.TicketType)
                                      .Include(p => p.ProjectPriority)
                                      .ToListAsync();
      return projects;

    }

    #endregion

    #region Get All Projects By Priority
    public async Task<List<Project>> GetAllProjectsByPriority(int companyId, string priorityName)
    {
      List<Project> projects = await GetAllProjectsByCompanyAsync(companyId);
      int priorityId = await LookupProjectPriorityId(priorityName);

      return projects.Where(p => p.ProjectPriorityId == priorityId).ToList();


    }

    #endregion

    #region Get Archived Projects By Company
    public async Task<List<Project>> GetArchivedProjectsByCompanyAsync(int companyId)
    {
      List<Project> projects = await _context.Projects.Where(p => p.CompanyId == companyId && p.Archived == true)
                                      .Include(p => p.Members)
                                      .Include(p => p.Tickets)
                                          .ThenInclude(t => t.Comments)
                                      .Include(p => p.Tickets)
                                          .ThenInclude(t => t.Attachments)
                                      .Include(p => p.Tickets)
                                          .ThenInclude(t => t.History)
                                      .Include(p => p.Tickets)
                                          .ThenInclude(t => t.DeveloperUser)
                                      .Include(p => p.Tickets)
                                          .ThenInclude(t => t.Notifications)
                                      .Include(p => p.Tickets)
                                          .ThenInclude(t => t.OwnerUser)
                                      .Include(p => p.Tickets)
                                          .ThenInclude(t => t.TicketStatus)
                                      .Include(p => p.Tickets)
                                          .ThenInclude(t => t.TicketPriority)
                                      .Include(p => p.Tickets)
                                          .ThenInclude(t => t.TicketType)
                                      .Include(p => p.ProjectPriority)
                                      .ToListAsync();

      return projects;
    }

    #endregion

    public Task<List<AppUser>> GetDevelopersOnProjectAsync(int projectId)
    {
      throw new NotImplementedException();
    }
    //CRUD - READ
    #region Get Project By Id
    public async Task<Project> GetProjectByIdAsync(int projectId, int companyId)
    {
      Project project = await _context.Projects
                                      .Include(p => p.Tickets)
                                          .ThenInclude(t => t.TicketPriority)
                                      .Include(p => p.Tickets)
                                          .ThenInclude(t => t.TicketStatus)
                                      .Include(p => p.Tickets)
                                          .ThenInclude(t => t.TicketType)
                                      .Include(p => p.Tickets)
                                          .ThenInclude(t => t.DeveloperUser)
                                      .Include(p => p.Tickets)
                                          .ThenInclude(t => t.OwnerUser)
                                      .Include(p => p.Members)
                                      .Include(p => p.ProjectPriority)
                                      .FirstOrDefaultAsync(p => p.Id == projectId && p.CompanyId == companyId);
      return project;
    }

    #endregion


    public async Task<AppUser> GetProjectManagerAsync(int projectId)
    {
      Project project = await _context.Projects
                                      .Include(p => p.Members)
                                      .FirstOrDefaultAsync(p => p.Id == projectId);
      //first or default gives back the first object or if the object doesn't exist it gives back an empty object

      foreach (AppUser member in project?.Members)
      {
        if (await _rolesService.IsUserInRoleAsync(member, Roles.ProjectManager.ToString()))
        {
          return member;
        }
      }
      return null;
    }


    public async Task<List<AppUser>> GetProjectMembersByRoleAsync(int projectId, string role)
    {
      Project project = await _context.Projects
                                      .Include(p => p.Members)
                                      .FirstOrDefaultAsync(p => p.Id == projectId);

      List<AppUser> members = new();

      foreach (var user in project.Members)
      {
        if (await _rolesService.IsUserInRoleAsync(user, role))
        {
          members.Add(user);
        }
      }
      return members;
    }

    public Task<List<AppUser>> GetSubmittersOnProjectAsync(int projectId)
    {
      throw new NotImplementedException();
    }

    public async Task<List<Project>> GetUserProjectsAsync(string userId)
    {
      try
      {
        List<Project> userProjects = (await _context.Users
                                                   .Include(u => u.Projects)
                                                        .ThenInclude(p => p.Company)
                                                   .Include(u => u.Projects)
                                                        .ThenInclude(p => p.Members)
                                                   .Include(u => u.Projects)
                                                        .ThenInclude(p => p.Tickets)
                                                   .Include(u => u.Projects)
                                                        .ThenInclude(t => t.Tickets)
                                                            .ThenInclude(t => t.DeveloperUser)
                                                    .Include(u => u.Projects)
                                                        .ThenInclude(t => t.Tickets)
                                                            .ThenInclude(t => t.OwnerUser)
                                                    .Include(u => u.Projects)
                                                        .ThenInclude(t => t.Tickets)
                                                            .ThenInclude(t => t.TicketPriority)
                                                    .Include(u => u.Projects)
                                                        .ThenInclude(t => t.Tickets)
                                                            .ThenInclude(t => t.TicketStatus)
                                                    .Include(u => u.Projects)
                                                        .ThenInclude(t => t.Tickets)
                                                            .ThenInclude(t => t.TicketType)
                                                    .FirstOrDefaultAsync(u => u.Id == userId)).Projects.ToList();

        return userProjects;

      }
      catch (Exception ex)
      {
        Console.WriteLine($"**** ERROR **** - Error Getting user Projects list. --> {ex.Message}");
        throw;
      }
    }


    #region Get Unassigned Projects
    public async Task<List<Project>> GetUnassignedProjectsAsync(int companyId)
    {
      List<Project> result = new();
      List<Project> projects = new();
      //want to see archived projects so 
      try
      {
        projects = await _context.Projects
                                 .Include(p => p.ProjectPriority)
                                 .Where(p => p.CompanyId == companyId)
                                 .ToListAsync();
        foreach (Project project in projects)
        {
          if ((await GetProjectMembersByRoleAsync(project.Id, nameof(Roles.ProjectManager))).Count == 0)
          {
            result.Add(project);
          }
        }

      }
      catch (Exception)
      {

        throw;
      }

      return result;
    }

    #endregion

    public async Task<List<AppUser>> GetUsersNotOnProjectAsync(int projectId, int companyId)
    {
      List<AppUser> users = await _context.Users.Where(u => u.Projects.All(p => p.Id != projectId)).ToListAsync();

      return users.Where(u => u.CompanyId == companyId).ToList();
    }

    public async Task<bool> IsAssignedProjectManagerAsync(string userId, int projectId)
    {
      try
      {
        string projectManagerId = (await GetProjectManagerAsync(projectId))?.Id;
        if (projectManagerId == userId)
        {
          return true;
        }
        else
        {
          return false;
        }
      }
      catch (Exception)
      {

        throw;
      }
    }

    public async Task<bool> IsUserOnProjectAsync(string userId, int projectId)
    {
      Project project = await _context.Projects
                                      .Include(p => p.Members)
                                      .FirstOrDefaultAsync(p => p.Id == projectId);
      bool result = false;
      if (project != null)
      {

        result = project.Members.Any(m => m.Id == userId);

      }

      return result;


    }

    public async Task<int> LookupProjectPriorityId(string priorityName)
    {
      int priorityId = (await _context.ProjectPriorities.FirstOrDefaultAsync(p => p.Name == priorityName)).Id;
      return priorityId;
    }

    public async Task RemoveProjectManagerAsync(int projectId)
    {
      Project project = await _context.Projects
                                      .Include(p => p.Members)
                                      .FirstOrDefaultAsync(p => p.Id == projectId);

      try
      {
        foreach (AppUser member in project?.Members)
        {
          if (await _rolesService.IsUserInRoleAsync(member, Roles.ProjectManager.ToString()))
          {
            await RemoveUserFromProjectAsync(member.Id, projectId);
          }
        }
      }
      catch
      {
        throw;
      }


    }

    public async Task RemoveUserFromProjectAsync(string userId, int projectId)
    {
      try
      {
        AppUser user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        Project project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == projectId);

        try
        {
          if (await IsUserOnProjectAsync(userId, projectId))
          {
            project.Members.Remove(user);
            await _context.SaveChangesAsync();
          }

        }
        catch (Exception)
        {
          throw;
        }


      }
      catch (Exception ex)
      {
        Console.WriteLine($"**** ERROR ***** - Error Removing User from project. --->  {ex.Message}");
      }
    }

    public async Task RemoveUsersFromProjectByRoleAsync(string role, int projectId)
    {
      try
      {
        List<AppUser> members = await GetProjectMembersByRoleAsync(projectId, role);
        Project project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == projectId);

        foreach (AppUser AppUser in members)
        {
          try
          {
            project.Members.Remove(AppUser);
            await _context.SaveChangesAsync();
          }
          catch (Exception)
          {
            throw;
          }
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine($"**** ERROR **** - Error Getting user Projects list. --> {ex.Message}");
        throw;
      }
    }

    public async Task RestoreProjectAsync(Project project)
    {
      try
      {
        project.Archived = false;
        await UpdateProjectAsync(project);

        //Archive the Tickets for the Project
        foreach (Ticket ticket in project.Tickets)
        {
          ticket.ArchivedByProject = false;
          _context.Update(ticket);
          await _context.SaveChangesAsync();
        }

      }
      catch (Exception)
      {

        throw;
      }
    }

    //CRUD - Update
    public async Task UpdateProjectAsync(Project project)
    {
      _context.Update(project);
      await _context.SaveChangesAsync();
    }

    public async Task<List<Project>> GetUnArchivedProjectsByCompanyAsync(int companyId)
    {
      List<Project> projects = await _context.Projects.Where(p => p.CompanyId == companyId && p.Archived == false)
                                            .Include(p => p.Members)
                                            .Include(p => p.Tickets)
                                                .ThenInclude(t => t.Comments)
                                            .Include(p => p.Tickets)
                                                .ThenInclude(t => t.Attachments)
                                            .Include(p => p.Tickets)
                                                .ThenInclude(t => t.History)
                                            .Include(p => p.Tickets)
                                                .ThenInclude(t => t.DeveloperUser)
                                            .Include(p => p.Tickets)
                                                .ThenInclude(t => t.Notifications)
                                            .Include(p => p.Tickets)
                                                .ThenInclude(t => t.OwnerUser)
                                            .Include(p => p.Tickets)
                                                .ThenInclude(t => t.TicketStatus)
                                            .Include(p => p.Tickets)
                                                .ThenInclude(t => t.TicketPriority)
                                            .Include(p => p.Tickets)
                                                .ThenInclude(t => t.TicketType)
                                            .Include(p => p.ProjectPriority)
                                            .ToListAsync();

      return projects;

    }
  }
}
