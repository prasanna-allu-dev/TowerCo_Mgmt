import { WORK_ORDER_STATUS_LABELS } from '@/features/workOrders/constants/workOrders';
import type { WorkOrder, WorkOrderPriority, WorkOrderStatus } from '@/features/workOrders/types';
import { dueLabel, dueTone, priorityTone, statusTone } from '@/features/workOrders/utils/workOrder';

export function PriorityPill({ priority }: { priority: WorkOrderPriority }) {
    return <span className={`pill tone-${priorityTone(priority)}`}>{priority}</span>;
}

export function WorkOrderStatusPill({ status }: { status: WorkOrderStatus }) {
    return <span className={`pill tone-${statusTone(status)}`}>{WORK_ORDER_STATUS_LABELS[status]}</span>;
}

export function DuePill({ workOrder }: { workOrder: WorkOrder }) {
    return <span className={`pill tone-${dueTone(workOrder)}`}>{dueLabel(workOrder)}</span>;
}
