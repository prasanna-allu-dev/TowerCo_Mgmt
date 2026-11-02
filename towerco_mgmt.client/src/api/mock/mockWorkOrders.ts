import { MOCK_CONFLICT_STATUS, MOCK_NOT_FOUND_STATUS, MOCK_VALIDATION_STATUS } from '@/constants/api';
import { MS_PER_DAY } from '@/constants/domain';
import {
    ASSIGNEE_REQUIRED_STATUS, MIN_TITLE_LENGTH, WORK_ORDER_CODE_DIGITS, WORK_ORDER_CODE_PREFIX, WORK_ORDER_TRANSITIONS,
} from '@/constants/workOrders';
import type { CreateWorkOrderRequest, UpdateWorkOrderStatusRequest, WorkOrder, WorkOrderFilter } from '@/types';
import { ApiError } from '../ApiError';
import type { WorkOrdersApi } from '../contracts';
import { delay } from './mockApi';
import { MOCK_SITES } from './mockData';
import type { MockSite } from './mockData';

// Fictional demo data, held in memory. Dates are relative to today so overdue and due-soon states always show.
type Seed = Pick<WorkOrder, 'siteId' | 'title' | 'description' | 'category' | 'priority' | 'status' | 'assignee'>
    & { createdDaysAgo: number; dueInDays: number };

const SEEDS: Seed[] = [
    { siteId: 1, title: 'Quarterly generator service', description: 'Oil, filters and load test on diesel generator.', category: 'Preventive', priority: 'Medium', status: 'Assigned', assignee: 'Arjun Mehta', createdDaysAgo: 10, dueInDays: 3 },
    { siteId: 1, title: 'Replace failed battery bank cells', description: 'Two cells below threshold on last check.', category: 'Corrective', priority: 'High', status: 'InProgress', assignee: 'Priya Nair', createdDaysAgo: 6, dueInDays: -2 },
    { siteId: 2, title: 'Install 5G AAU for Skyline Mobile', description: 'New tenancy equipment at 32 m.', category: 'Installation', priority: 'High', status: 'Open', assignee: null, createdDaysAgo: 2, dueInDays: 12 },
    { siteId: 4, title: 'Annual structural inspection', description: 'Bolt torque, corrosion and foundation check.', category: 'Inspection', priority: 'Medium', status: 'Completed', assignee: 'Thandi Nkosi', createdDaysAgo: 40, dueInDays: -20 },
    { siteId: 4, title: 'Air conditioner not cooling', description: 'Shelter temperature alarm raised overnight.', category: 'Corrective', priority: 'Critical', status: 'Open', assignee: null, createdDaysAgo: 1, dueInDays: 1 },
    { siteId: 7, title: 'Security camera offline', description: 'Camera 2 not streaming to NOC.', category: 'Corrective', priority: 'Low', status: 'OnHold', assignee: 'Wanjiru Kamau', createdDaysAgo: 15, dueInDays: -5 },
    { siteId: 8, title: 'Microwave dish realignment', description: 'Link margin degraded after storm.', category: 'Corrective', priority: 'High', status: 'Assigned', assignee: 'Wanjiru Kamau', createdDaysAgo: 3, dueInDays: 4 },
    { siteId: 10, title: 'Solar array panel cleaning', description: 'Dust build up reducing yield.', category: 'Preventive', priority: 'Low', status: 'Open', assignee: null, createdDaysAgo: 5, dueInDays: 20 },
    { siteId: 11, title: 'Lightning protection test', description: 'Earth resistance measurement.', category: 'Inspection', priority: 'Medium', status: 'Cancelled', assignee: null, createdDaysAgo: 30, dueInDays: -10 },
    { siteId: 11, title: 'Generator fuel top up', description: 'Tank at 20 percent.', category: 'Preventive', priority: 'High', status: 'InProgress', assignee: 'Chidi Okafor', createdDaysAgo: 2, dueInDays: 0 },
];

const isoDaysFromNow = (days: number) => new Date(Date.now() + days * MS_PER_DAY).toISOString();
const dateDaysFromNow = (days: number) => isoDaysFromNow(days).slice(0, 10);
const codeFor = (id: number) => `${WORK_ORDER_CODE_PREFIX}${String(id).padStart(WORK_ORDER_CODE_DIGITS, '0')}`;

function findSite(siteId: number): MockSite {
    const site = MOCK_SITES.find(s => s.id === siteId);
    if (!site) throw new ApiError(`Site ${siteId} was not found.`, MOCK_NOT_FOUND_STATUS);
    return site;
}

function siteFields(siteId: number) {
    const { siteCode, name, countryCode } = findSite(siteId);
    return { siteId, siteCode, siteName: name, countryCode };
}

function fromSeed({ createdDaysAgo, dueInDays, ...seed }: Seed, index: number): WorkOrder {
    const id = index + 1;
    const completedAtUtc = seed.status === 'Completed' ? isoDaysFromNow(dueInDays) : null;
    return {
        ...seed, ...siteFields(seed.siteId), id, code: codeFor(id),
        createdAtUtc: isoDaysFromNow(-createdDaysAgo), dueDate: dateDaysFromNow(dueInDays), completedAtUtc,
    };
}

const workOrders: WorkOrder[] = SEEDS.map(fromSeed);

function matches(wo: WorkOrder, filter: WorkOrderFilter): boolean {
    const text = filter.search?.toLowerCase();
    const textHit = !text || [wo.code, wo.title, wo.siteCode, wo.siteName].some(v => v.toLowerCase().includes(text));
    return textHit && (!filter.status || wo.status === filter.status)
        && (!filter.priority || wo.priority === filter.priority) && (!filter.country || wo.countryCode === filter.country);
}

function findWorkOrder(id: number): WorkOrder {
    const wo = workOrders.find(w => w.id === id);
    if (!wo) throw new ApiError(`Work order ${id} was not found.`, MOCK_NOT_FOUND_STATUS);
    return wo;
}

function create(request: CreateWorkOrderRequest): WorkOrder {
    if (request.title.trim().length < MIN_TITLE_LENGTH)
        throw new ApiError(`Title must be at least ${MIN_TITLE_LENGTH} characters.`, MOCK_VALIDATION_STATUS);
    const id = Math.max(0, ...workOrders.map(w => w.id)) + 1;
    const status = request.assignee ? ASSIGNEE_REQUIRED_STATUS : 'Open';
    const wo: WorkOrder = {
        ...request, ...siteFields(request.siteId), id, code: codeFor(id), status,
        createdAtUtc: new Date().toISOString(), completedAtUtc: null,
    };
    workOrders.unshift(wo);
    return { ...wo };
}

function assertTransition(wo: WorkOrder, request: UpdateWorkOrderStatusRequest): void {
    if (!WORK_ORDER_TRANSITIONS[wo.status].includes(request.status))
        throw new ApiError(`Cannot move ${wo.code} from ${wo.status} to ${request.status}.`, MOCK_CONFLICT_STATUS);
    if (request.status === ASSIGNEE_REQUIRED_STATUS && !request.assignee)
        throw new ApiError('Select an engineer before assigning.', MOCK_VALIDATION_STATUS);
}

function updateStatus(id: number, request: UpdateWorkOrderStatusRequest): WorkOrder {
    const wo = findWorkOrder(id);
    assertTransition(wo, request);
    wo.status = request.status;
    if (request.assignee !== undefined) wo.assignee = request.assignee;
    wo.completedAtUtc = request.status === 'Completed' ? new Date().toISOString() : null;
    return { ...wo };
}

export const mockWorkOrdersApi: WorkOrdersApi = {
    list: (filter = {}) => delay(() => workOrders.filter(w => matches(w, filter)).map(w => ({ ...w }))),
    get: id => delay(() => ({ ...findWorkOrder(id) })),
    create: request => delay(() => create(request)),
    updateStatus: (id, request) => delay(() => updateStatus(id, request)),
};
