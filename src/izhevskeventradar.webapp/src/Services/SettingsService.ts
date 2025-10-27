import type { IGroup } from "../Interfaces/Groups/IGroup";
import type { IGroupPatch } from "../Interfaces/Groups/IGroupPatch";
import type { IGroupValidationResult } from "../Interfaces/Groups/IGroupValidationResult";
import { request } from "../Utils/api";

const BASE_URL = '/api/groups';

export const fetchGroups = (signal?: AbortSignal): Promise<IGroup[]> =>
    request<IGroup[]>(BASE_URL, { method: 'GET', signal });

export const createGroup = async (internalId: string): Promise<IGroup> =>
    request<IGroup>(BASE_URL, { method: 'POST', body: internalId });

export const updateGroup = async (id: number, groupPatch: IGroupPatch): Promise<IGroup> =>
    request<IGroup>(`${BASE_URL}/${id}`, { method: 'PATCH', body: groupPatch });

export const deleteGroup = async (id: number): Promise<void> =>
    request<void>(`${BASE_URL}/${id}`, { method: 'DELETE' });

export const validateGroup = async (internalId: string): Promise<IGroupValidationResult> =>
    request<IGroupValidationResult>(`${BASE_URL}/${internalId}/validate`, { method: 'POST' });