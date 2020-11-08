﻿<%@ Assembly Name="ASC.Core.Common" %>
<%@ Assembly Name="ASC.Web.Files" %>

<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Tree.ascx.cs" Inherits="ASC.Web.Files.Controls.Tree" %>
<%@ Import Namespace="ASC.Core" %>
<%@ Import Namespace="ASC.Files.Core.Security" %>
<%@ Import Namespace="ASC.Web.Core.Files" %>
<%@ Import Namespace="ASC.Web.Files.Classes" %>
<%@ Import Namespace="ASC.Web.Files.Resources" %>
<%@ Import Namespace="ASC.Data.Storage" %>

<%-- TREE --%>
<div id="<%= ID %>" class="jstree webkit-scrollbar <%= AdditionalCssClass %>">
    <ul>
        <% if (!IsVisitor)
           { %>
        <li data-id="<%= Global.FolderMy %>" class="tree-node jstree-closed">
            <div class="jstree-wholerow">&nbsp;</div>
            <span class="jstree-icon jstree-expander"></span>
            <a data-id="<%= Global.FolderMy %>" title="<%= FilesUCResource.MyFiles %>" href="<%= FilesLinkUtility.FilesBaseAbsolutePath + "#" + Global.FolderMy %>">
                <span class="menu-item-icon myFiles"><svg class="menu-item-svg"><use base="<%= WebPath.GetPath("/")%>" href="/skins/default/images/svg/documents-icons.svg#documentsIconspeople"></use></svg></span><%= FilesUCResource.MyFiles %>
                <input type="hidden" name="entry_data" data-id="<%= Global.FolderMy %>" data-entryType="folder" data-access="<%= (int)FileShare.None %>" />
            </a>
            <% if (WithNew)
               { %>
            <span class="new-label-menu is-new" title="<%= FilesUCResource.RemoveIsNew %>" data-id="<%= Global.FolderMy %>"></span>
            <% } %>
        </li>
        <% } %>

        <% if (!CoreContext.Configuration.Personal)
           { %>

        <% if (!Global.IsOutsider)
           { %>
        <li data-id="<%= Global.FolderShare %>" class="tree-node jstree-closed access-read">
            <div class="jstree-wholerow">&nbsp;</div>
            <span class="jstree-icon jstree-expander"></span>
            <a data-id="<%= Global.FolderShare %>" title="<%= FilesUCResource.SharedForMe %>" href="<%= FilesLinkUtility.FilesBaseAbsolutePath + "#" + Global.FolderShare %>">
                <span class="menu-item-icon shareformeFiles"><svg class="menu-item-svg"><use base="<%= WebPath.GetPath("/")%>" href="/skins/default/images/svg/documents-icons.svg#documentsIconsfeed"></use></svg></span><%= FilesUCResource.SharedForMe %>
                <input type="hidden" name="entry_data" data-id="<%= Global.FolderShare %>" data-entryType="folder" data-access="<%= (int)FileShare.Read %>" />
            </a>
            <% if (WithNew)
               { %>
            <span class="new-label-menu is-new" title="<%= FilesUCResource.RemoveIsNew %>" data-id="<%= Global.FolderShare %>"></span>
            <% } %>
        </li>
        <% } %>

        <li data-id="<%= Global.FolderCommon %>" class="tree-node jstree-closed <%= Global.IsAdministrator ? string.Empty : "access-read" %>">
            <div class="jstree-wholerow">&nbsp;</div>
            <span class="jstree-icon jstree-expander"></span>
            <a data-id="<%= Global.FolderCommon %>" title="<%= FilesUCResource.CorporateFiles %>" href="<%= FilesLinkUtility.FilesBaseAbsolutePath + "#" + Global.FolderCommon %>">
                <span class="menu-item-icon corporateFiles"><svg class="menu-item-svg"><use base="<%= WebPath.GetPath("/")%>" href="/skins/default/images/svg/documents-icons.svg#documentsIconscase"></use></svg></span><%= FilesUCResource.CorporateFiles %>
                <input type="hidden" name="entry_data" data-id="<%= Global.FolderCommon %>" data-entryType="folder" data-access="<%= (int)(Global.IsAdministrator ? FileShare.ReadWrite : FileShare.Read) %>" />
            </a>
            <% if (WithNew)
               { %>
            <span class="new-label-menu is-new" title="<%= FilesUCResource.RemoveIsNew %>" data-id="<%= Global.FolderCommon %>"></span>
            <% } %>
        </li>

        <% if (FolderIDCurrentRoot == null)
           { %>

        <% if (Global.FolderProjects != null)
           { %>
        <li data-id="<%= Global.FolderProjects %>" class="tree-node jstree-closed access-read">
            <div class="jstree-wholerow">&nbsp;</div>
            <span class="jstree-icon jstree-expander"></span>
            <a data-id="<%= Global.FolderProjects %>" title="<%= FilesUCResource.ProjectFiles %>" href="<%= FilesLinkUtility.FilesBaseAbsolutePath + "#" + Global.FolderProjects %>">
                <span class="menu-item-icon projectFiles"><svg class="menu-item-svg"><use base="<%= WebPath.GetPath("/")%>" href="/skins/default/images/svg/documents-icons.svg#documentsIconsprojects"></use></svg></span><%= FilesUCResource.ProjectFiles %>
                <input type="hidden" name="entry_data" data-id="<%= Global.FolderProjects %>" data-entryType="folder" data-access="<%= (int)FileShare.Read %>" />
            </a>
            <% if (WithNew)
               { %>
            <span class="new-label-menu is-new" title="<%= FilesUCResource.RemoveIsNew %>" data-id="<%= Global.FolderProjects %>"></span>
            <% } %>
        </li>
        <% } %>

        <% }
           else
           { %>
        <li data-id="<%= FolderIDCurrentRoot %>" class="tree-node jstree-closed">
            <div class="jstree-wholerow">&nbsp;</div>
            <span class="jstree-icon jstree-expander"></span>
            <a data-id="<%= FolderIDCurrentRoot %>" title="<%= FilesUCResource.ProjectFiles %>" href="<%= FilesLinkUtility.FilesBaseAbsolutePath + "#" + FolderIDCurrentRoot %>">
                <span class="menu-item-icon projectFiles"><svg class="menu-item-svg"><use base="<%= WebPath.GetPath("/")%>" href="/skins/default/images/svg/documents-icons.svg#documentsIconsprojects"></use></svg></span><%= FilesUCResource.ProjectFiles %>
                <input type="hidden" name="entry_data" data-id="<%= FolderIDCurrentRoot %>" data-entryType="folder" data-access="<%= (int)FileShare.ReadWrite %>" />
            </a>
        </li>
        <% } %>

        <% } %>

        <% if (!WithoutTrash)
           { %>
        <li data-id="<%= Global.FolderTrash %>" class="tree-node jstree-closed">
            <div class="jstree-wholerow">&nbsp;</div>
            <span class="jstree-icon visibility-hidden"></span>
            <a data-id="<%= Global.FolderTrash %>" title="<%= FilesUCResource.Trash %>" href="<%= FilesLinkUtility.FilesBaseAbsolutePath + "#" + Global.FolderTrash %>">
                <span class="menu-item-icon trashFiles"><svg class="menu-item-svg"><use base="<%= WebPath.GetPath("/")%>" href="/skins/default/images/svg/documents-icons.svg#documentsIconstrash"></use></svg></span><%= FilesUCResource.Trash %>
                <input type="hidden" name="entry_data" data-id="<%= Global.FolderTrash %>" data-entryType="folder" data-access="<%= (int)FileShare.ReadWrite %>" />
            </a>
        </li>
        <% } %>
    </ul>
</div>
