import { MS_PER_DAY } from '@/shared/constants/domain';
import { CLOSED_WORK_ORDER_STATUSES, DUE_SOON_DAYS } from '@/features/workOrders/constants/workOrders';
import type { Severity } from '@/shared/types/domain';
import type { WorkOrder, WorkOrderPriority, WorkOrderStatus } from '@/features/workOrders/types';

const PRIORITY_SEVERITY: Record<WorkOrderPriority, Severity> = {
    Low: 'neutral', Medium: 'ok', High: 'warning', Critical: 'critical',
};

const STATUS_SEVERITY: Record<WorkOrderStatus, Severity> = {
    Open: 'neutral', Assigned: 'neutral', InProgress: 'warning', OnHold: 'warning', Completed: 'ok', Cancelled: 'critical',
};

export const priorityTone = (priority: WorkOrderPriority): Severity => PRIORITY_SEVERITY[priority];
export const statusTone = (status: WorkOrderStatus): Severity => STATUS_SEVERITY[status];
export const isClosed = (wo: WorkOrder): boolean => CLOSED_WORK_ORDER_STATUSES.includes(wo.status);

function startOfToday(): number {
    const today = new Date();
    today.setHours(0, 0, 0, 0);
    return today.getTime();
}

export const daysUntilDue = (wo: WorkOrder): number =>
    Math.round((new Date(`${wo.dueDate}T00:00:00`).getTime() - startOfToday()) / MS_PER_DAY);

export const isOverdue = (wo: WorkOrder): boolean => !isClosed(wo) && daysUntilDue(wo) < 0;
export const isDueSoon = (wo: WorkOrder): boolean => !isClosed(wo) && daysUntilDue(wo) >= 0 && daysUntilDue(wo) <= DUE_SOON_DAYS;

export function dueTone(wo: WorkOrder): Severity {
    if (isOverdue(wo)) return 'critical';
    return isDueSoon(wo) ? 'warning' : 'ok';
}

export function dueLabel(wo: WorkOrder): string {
    if (isClosed(wo)) return wo.dueDate;
    const days = daysUntilDue(wo);
    if (days < 0) return `${-days}d overdue`;
    return days === 0 ? 'Due today' : `Due in ${days}d`;
}
