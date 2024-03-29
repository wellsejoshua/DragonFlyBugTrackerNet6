﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DragonFlyBugTrackerNet6.Data;
using DragonFlyBugTrackerNet6.Models;
using DragonFlyBugTrackerNet6.Models.Enums;
using DragonFlyBugTrackerNet6.Models.ViewModels;
using DragonFlyBugTrackerNet6.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using DragonFlyBugTrackerNet6.Extensions;

namespace DragonFlyBugTrackerNet6.Controllers
{
    [Authorize]
    public class ProjectsController : Controller
    {
        #region Properties
        private readonly IBTRolesService _rolesService;
        private readonly IBTLookupService _lookupService;
        private readonly IBTFileService _fileService;
        private readonly IBTProjectService _projectService;
        private readonly IBTCompanyInfoService _companyInfoService;
        private readonly UserManager<AppUser> _userManager;
        #endregion

        #region Constructor
        public ProjectsController(IBTRolesService rolesService,
                                  IBTLookupService lookupService,
                                  IBTFileService fileService,
                                  IBTProjectService projectService,
                                  IBTCompanyInfoService companyInfoService,
                                  UserManager<AppUser> userManager)
        {
            _rolesService = rolesService;
            _lookupService = lookupService;
            _fileService = fileService;
            _projectService = projectService;
            _companyInfoService = companyInfoService;
            _userManager = userManager;
        }
        #endregion

        #region My Projects
        public async Task<IActionResult> MyProjects()
        {
            string userId = _userManager.GetUserId(User);

            List<Project> projects = await _projectService.GetUserProjectsAsync(userId);

            return View(projects);
        }

        #endregion

        #region All Projects
        public async Task<IActionResult> AllProjects()
        {

            List<Project> projects = new();
            int companyId = User.Identity.GetCompanyId().Value;



            if (User.IsInRole(nameof(Roles.Admin)) || User.IsInRole(nameof(Roles.ProjectManager)))
            {
                projects = await _companyInfoService.GetAllProjectsAsync(companyId);
            }
            else
            {
                projects = await _projectService.GetAllProjectsByCompanyAsync(companyId);
            }

            return View(projects);
        }

        #endregion

        #region Archived Projects

        public async Task<IActionResult> ArchivedProjects()
        {
            int companyId = User.Identity.GetCompanyId().Value;

            List<Project> projects = await _projectService.GetArchivedProjectsByCompanyAsync(companyId);

            return View(projects);
        }

        #endregion

        #region Current / UnArchived Projects

        public async Task<IActionResult> ActiveProjects()
        {
            int companyId = User.Identity.GetCompanyId().Value;

            List<Project> projects = await _projectService.GetUnArchivedProjectsByCompanyAsync(companyId);

            return View(projects);
        }

        #endregion

        #region Unassigned Projects
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UnassignedProjects()
        {
            int companyId = User.Identity.GetCompanyId().Value;

            List<Project> projects = new();

            projects = await _projectService.GetUnassignedProjectsAsync(companyId);

            return View(projects);
        }

        #endregion

        #region Assign PM
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> AssignPM(int projectId)
        {
            int companyId = User.Identity.GetCompanyId().Value;

            AssignPMViewModel model = new();
            model.Project = await _projectService.GetProjectByIdAsync(projectId, companyId);
            model.PMList = new SelectList(await _rolesService.GetUsersInRoleAsync(nameof(Roles.ProjectManager), companyId), "Id", "FullName");
            AppUser manager = await _projectService.GetProjectManagerAsync(projectId);
            if (manager != null)
            {
                model.PMID = manager.Id;
            }
            return View(model);
        }
        #endregion

        #region Assign PM Post
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignPM(AssignPMViewModel model)
        {
            if (!string.IsNullOrEmpty(model.PMID))
            {

                await _projectService.AddProjectManagerAsync(model.PMID, model.Project.Id);

                return RedirectToAction(nameof(Details), new { id = model.Project.Id });

            }

            return RedirectToAction(nameof(AssignPM), new { projectId = model.Project.Id });
        }
        #endregion

        #region Project Select Get
        [Authorize(Roles = "Admin,ProjectManager")]
        [HttpGet]
        public async Task<IActionResult> ProjectSelect()
        {


            int companyId = User.Identity.GetCompanyId().Value;

            if (User.IsInRole(nameof(Roles.Admin)) || User.IsInRole(nameof(Roles.ProjectManager)))
            {
                ViewData["Id"] = new SelectList(await _projectService.GetAllProjectsByCompanyAsync(companyId), "Id", "Name");
            }
            else
            {
                List<Project> projects = new();
                ViewData["Id"] = new SelectList(projects, "Id", "Name");
            }

            return View();
        }

        #endregion

        #region Project Select Post
        [Authorize(Roles = "Admin,ProjectManager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProjectSelect(int id)
        {
            int companyId = User.Identity.GetCompanyId().Value;
            Project project = await _projectService.GetProjectByIdAsync(id, companyId);

            return RedirectToAction("AssignMembers", "Projects", new { id = id });

        }

        #endregion

        #region AssignPmProjectSelect Get

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> AssignPmProjectSelect()
        {


            int companyId = User.Identity.GetCompanyId().Value;


            if (User.IsInRole(nameof(Roles.Admin)) || User.IsInRole(nameof(Roles.ProjectManager)))
            {
                ViewData["Id"] = new SelectList(await _projectService.GetAllProjectsByCompanyAsync(companyId), "Id", "Name");
            }
            else
            {
                List<Project> projects = new();
                ViewData["Id"] = new SelectList(projects, "Id", "Name");
            }

            return View();
        }

        #endregion

        #region AssignPmProjectSelect Post
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignPmProjectSelect(int id)
        {
            int companyId = User.Identity.GetCompanyId().Value;

            AssignPMViewModel model = new();
            model.Project = await _projectService.GetProjectByIdAsync(id, companyId);
            model.PMList = new SelectList(await _rolesService.GetUsersInRoleAsync(nameof(Roles.ProjectManager), companyId), "Id", "FullName");



            return RedirectToAction("AssignPM", "Projects", new { projectId = id });

        }

        #endregion

        #region Assign Members Get
        [Authorize(Roles = "Admin,ProjectManager")]
        [HttpGet]
        public async Task<IActionResult> AssignMembers(int id)
        {
            ProjectMembersViewModel model = new();
            int companyId = User.Identity.GetCompanyId().Value;

            model.Project = await _projectService.GetProjectByIdAsync(id, companyId);
            List<AppUser> developers = await _rolesService.GetUsersInRoleAsync(nameof(Roles.Developer), companyId);
            List<AppUser> submitters = await _rolesService.GetUsersInRoleAsync(nameof(Roles.Submitter), companyId);
            //Concat has to be of same exact type. Concat converts to an I enumerable so has to be converted back to a list
            List<AppUser> compamyMembers = developers.Concat(submitters).ToList();

            List<string> projectMembers = model.Project.Members.Select(m => m.Id).ToList();
            model.Users = new MultiSelectList(compamyMembers, "Id", "FullName", projectMembers);

            return View(model);


        }
        #endregion

        #region Assign Members Post
        [Authorize(Roles = "Admin,ProjectManager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignMembers(ProjectMembersViewModel model)
        {
            if (model.SelectedUsers != null)
            {
                //select allows you to look at one particular column
                List<string> memberIds = (await _projectService.GetAllProjectMembersExceptPMAsync(model.Project.Id)).Select(m => m.Id).ToList();

                //remove current members 
                foreach (string member in memberIds)
                {
                    await _projectService.RemoveUserFromProjectAsync(member, model.Project.Id);
                }

                //add selected members
                foreach (string member in model.SelectedUsers)
                {
                    await _projectService.AddUserToProjectAsync(member, model.Project.Id);
                }

                //got to project details
                return RedirectToAction("Details", "Projects", new { id = model.Project.Id });

            }

            return RedirectToAction(nameof(AssignMembers), new { id = model.Project.Id });
        }

        #endregion

        #region Details
        // GET: Projects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            int companyId = User.Identity.GetCompanyId().Value;

            Project project = await _projectService.GetProjectByIdAsync(id.Value, companyId);




            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        #endregion

        #region Create
        // GET: Projects/Create
        [Authorize(Roles = "Admin,ProjectManager")]
        public async Task<IActionResult> Create()
        {
            int companyId = User.Identity.GetCompanyId().Value;
            //Add ViewModel Instance 
            AddProjectWithPMViewModel model = new();

            //Load Sectlist with data ie. PMList and PriorityList
            model.PMList = new SelectList(await _rolesService.GetUsersInRoleAsync(Roles.ProjectManager.ToString(), companyId), "Id", "FullName");
            model.PriorityList = new SelectList(await _lookupService.GetProjectPrioritiesAsync(), "Id", "Name");

            return View(model);
        }

        #endregion

        #region Create Post
        // POST: Projects/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin,ProjectManager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddProjectWithPMViewModel model)
        {
            if (model != null)
            {
                int companyId = User.Identity.GetCompanyId().Value;
                try
                {
                    if (model.Project.ImageFormFile != null)
                    {
                        model.Project.ImageData = await _fileService.ConvertFileToByteArrayAsync(model.Project.ImageFormFile);
                        model.Project.ImageFileName = model.Project.ImageFormFile.FileName;
                        model.Project.ImageFileContentType = model.Project.ImageFormFile.ContentType;
                    }

                    model.Project.CompanyId = companyId;

                    await _projectService.AddNewProjectAsync(model.Project);

                    if (!string.IsNullOrEmpty(model.PmId))
                    {
                        await _projectService.AddProjectManagerAsync(model.PmId, model.Project.Id);
                    }
                }
                catch (Exception)
                {

                    throw;
                }
                //Todo redirect to all projects
                return RedirectToAction(nameof(AllProjects));
            }

            //add viewdata
            return RedirectToAction("Create");
        }

        #endregion

        #region Edit
        // GET: Projects/Edit/5
        [Authorize(Roles = "Admin,ProjectManager")]
        public async Task<IActionResult> Edit(int? id)
        {
            int companyId = User.Identity.GetCompanyId().Value;
            //Add ViewModel Instance 
            AddProjectWithPMViewModel model = new();

            model.Project = await _projectService.GetProjectByIdAsync(id.Value, companyId);

            //Load Sectlist with data ie. PMList and PriorityList
            model.PMList = new SelectList(await _rolesService.GetUsersInRoleAsync(Roles.ProjectManager.ToString(), companyId), "Id", "FullName");
            model.PriorityList = new SelectList(await _lookupService.GetProjectPrioritiesAsync(), "Id", "Name");

            return View(model);
        }

        #endregion

        #region Edit Post

        // POST: Projects/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin,ProjectManager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AddProjectWithPMViewModel model)
        {
            if (model != null)
            {
                try
                {
                    if (model.Project.ImageFormFile != null)
                    {
                        model.Project.ImageData = await _fileService.ConvertFileToByteArrayAsync(model.Project.ImageFormFile);
                        model.Project.ImageFileName = model.Project.ImageFormFile.FileName;
                        model.Project.ImageFileContentType = model.Project.ImageFormFile.ContentType;
                    }

                    await _projectService.UpdateProjectAsync(model.Project);

                    if (!string.IsNullOrEmpty(model.PmId))
                    {
                        await _projectService.AddProjectManagerAsync(model.PmId, model.Project.Id);
                    }

                    //Might not need to be added
                    return RedirectToAction("AllProjects");
                }
                catch (DbUpdateConcurrencyException)
                {

                    if (!await ProjectExists(model.Project.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return RedirectToAction("Edit");
        }

        #endregion

        #region Archive
        // GET: Projects/Archive/5
        [Authorize(Roles = "Admin,ProjectManager")]
        public async Task<IActionResult> Archive(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            int companyId = User.Identity.GetCompanyId().Value;
            var project = await _projectService.GetProjectByIdAsync(id.Value, companyId);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        #endregion

        #region Archive Post
        // POST: Projects/Archive/5
        [Authorize(Roles = "Admin,ProjectManager")]
        [HttpPost, ActionName("Archive")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ArchiveConfirmed(int id)
        {
            int companyId = User.Identity.GetCompanyId().Value;
            var project = await _projectService.GetProjectByIdAsync(id, companyId);

            await _projectService.ArchiveProjectAsync(project);

            return RedirectToAction("ArchivedProjects");
        }

        #endregion

        #region Restore
        // GET: Projects/Restore/5
        [Authorize(Roles = "Admin,ProjectManager")]
        public async Task<IActionResult> Restore(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            int companyId = User.Identity.GetCompanyId().Value;
            var project = await _projectService.GetProjectByIdAsync(id.Value, companyId);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        #endregion

        #region Restore Post
        // POST: Projects/Restore/5
        [Authorize(Roles = "Admin,ProjectManager")]
        [HttpPost, ActionName("Restore")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RestoreConfirmed(int id)
        {
            int companyId = User.Identity.GetCompanyId().Value;
            var project = await _projectService.GetProjectByIdAsync(id, companyId);

            await _projectService.RestoreProjectAsync(project);

            return RedirectToAction("AllProjects");
        }

        #endregion

        #region Project Exists Private
        private async Task<bool> ProjectExists(int id)
        {
            int companyId = User.Identity.GetCompanyId().Value;

            return (await _projectService.GetAllProjectsByCompanyAsync(companyId)).Any(p => p.Id == id);
        }

        #endregion
    }

}
