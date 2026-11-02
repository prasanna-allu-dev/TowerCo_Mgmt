import type { WORK_ORDER_CATEGORIES, WORK_ORDER_PRIORITIES, WORK_ORDER_STATUSES } from '@/constants/workOrders';

export type WorkOrderStatus = (typeof WORK_ORDER_STATUSES)[number];
export type WorkOrderPriority = (typeof WORK_ORDER_PRIORITIES)[number];
export type WorkOrderCategory = (typeof WORK_ORDER_CATEGORIES)[number];

export interface WorkOrder {
    id: number;
    code: string;
    siteId: number;
    siteCode: string;
    siteName: string;
    countryCode: string;
    title: string;
    description: string;
    category: WorkOrderCategory;
    priority: WorkOrderPriority;
    status: WorkOrderStatus;
    assignee: string | null;
    createdAtUtc: string;
    dueDate: string;
    completedAtUtc: string | null;
}

export interface WorkOrderFilter {
    search?: string;
    status?: string;
    priority?: string;
    country?: string;
}

export interface CreateWorkOrderRequest {
    siteId: number;
    title: string;
    description: string;
    category: WorkOrderCategory;
    priority: WorkOrderPriority;
    dueDate: string;
    assignee: string | null;
}

export interface UpdateWorkOrderStatusRequest {
    status: WorkOrderStatus;
    assignee?: string | null;
}
