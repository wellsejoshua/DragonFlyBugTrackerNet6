﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DragonFlyBugTrackerNet6.Data;
using DragonFlyBugTrackerNet6.Models;
using DragonFlyBugTrackerNet6.Services.Interfaces;

namespace DragonFlyBugTrackerNet6.Services
{
  public class BTInviteService : IBTInviteService
  {
    private readonly ApplicationDbContext _context;

    public BTInviteService(ApplicationDbContext context)
    {
      _context = context;
    }


    #region Accept Invite
    public async Task<bool> AcceptInviteAsync(Guid? token, string userId, int companyId)
    {
      Invite invite = await _context.Invites.FirstOrDefaultAsync(i => i.CompanyToken == token);
      if (invite == null)
      {
        return false;
      }

      try
      {
        invite.IsValid = false;
        invite.InviteeId = userId;
        await _context.SaveChangesAsync();

        return true;
      }
      catch (Exception)
      {

        throw;
      }
    }

    #endregion

    #region Add New Invite

    public async Task AddNewInviteAsync(Invite invite)
    {
      try
      {
        await _context.Invites.AddAsync(invite);
        await _context.SaveChangesAsync();
      }
      catch (Exception)
      {

        throw;
      }
    }

    #endregion

    #region Any Invite
    public async Task<bool> AnyInviteAsync(Guid token, string email, int companyId)
    {
      try
      {
        bool result = await _context.Invites.Where(i => i.CompanyId == companyId)
                                            .AnyAsync(i => i.CompanyToken == token && i.InviteeEmail == email);
        return result;
      }
      catch (Exception)
      {

        throw;
      }
    }

    #endregion

    #region Get Invite
    public async Task<Invite> GetInviteAsync(int inviteId, int companyId)
    {
      try
      {
        Invite invite = await _context.Invites.Where(i => i.CompanyId == companyId)
                                              .Include(i => i.Company)
                                              .Include(i => i.Project)
                                              .Include(i => i.Invitor)
                                              .FirstOrDefaultAsync(i => i.Id == inviteId);
        return invite;
      }
      catch (Exception)
      {

        throw;
      }
    }

    #endregion

    #region Get Invite Overload
    public async Task<Invite> GetInviteAsync(Guid token, string email, int companyId)
    {
      try
      {
        Invite invite = await _context.Invites.Where(i => i.CompanyId == companyId)
                                              .Include(i => i.Company)
                                              .Include(i => i.Project)
                                              .Include(i => i.Invitor)
                                              .FirstOrDefaultAsync(i => i.CompanyToken == token && i.InviteeEmail == email);
        return invite;
      }
      catch (Exception)
      {

        throw;
      }

    }

    #endregion

    #region Validate Invite Code
    public async Task<bool> ValidateInviteCodeAsync(Guid? token)
    {
      if (token == null)
      {
        return false;
      }

      bool result = false;

      Invite invite = await _context.Invites.FirstOrDefaultAsync(i => i.CompanyToken == token);
      if (invite != null)
      {
        //Determine the date
        DateTime inviteDate = invite.InviteDate.DateTime;

        //custom validation of invite based on the date it was issued
        //In this case I'm allowing an invite to be valid for 7 days
        bool validDate = (DateTime.Now - inviteDate).TotalDays <= 7;

        if (validDate)
        {
          result = invite.IsValid;
        }

      }
      return result;

    }

    #endregion
  }
}
