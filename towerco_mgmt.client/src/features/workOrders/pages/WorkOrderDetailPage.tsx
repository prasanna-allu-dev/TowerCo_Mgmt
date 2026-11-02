import { ROUTES, sitePath } from '@/shared/constants/routes';
import { useState } from 'react';
import { Link, useParams } from 'react-router-dom';
import { workOrdersApi } from '@/features/workOrders/api';
import { Card, StateMessage } from '@/shared/components';
import { DuePill, PriorityPill, WorkOrderStatusActions, WorkOrderStatusPill } from '@/features/workOrders/components';
import '@/features/workOrders/components/workOrders.css';
import { DISPLAY_LOCALE } from '@/shared/constants/domain';
import { useApi } from '@/shared/hooks/useApi';
import PageHeader from '@/shared/components/PageHeader';
import type { WorkOrder } from '@/features/workOrders/types';

const formatDateTime = (iso: string | null) => (iso ? new Date(iso).toLocaleString(DISPLAY_LOCALE) : '-');

function WorkOrderDetails({ wo }: { wo: WorkOrder }) {
    return (
        <dl className="details">
            <dt>Site</dt><dd><Link to={sitePath(wo.siteId)}>{wo.siteCode} · {wo.siteName}</Link></dd>
            <dt>Category</dt><dd>{wo.category}</dd>
            <dt>Priority</dt><dd><PriorityPill priority={wo.priority} /></dd>
            <dt>Status</dt><dd><WorkOrderStatusPill status={wo.status} /></dd>
            <dt>Assignee</dt><dd>{wo.assignee ?? 'Unassigned'}</dd>
            <dt>Due</dt><dd>{wo.dueDate} <DuePill workOrder={wo} /></dd>
            <dt>Created</dt><dd>{formatDateTime(wo.createdAtUtc)}</dd>
            <dt>Completed</dt><dd>{formatDateTime(wo.completedAtUtc)}</dd>
            <dt>Description</dt><dd>{wo.description || '-'}</dd>
        </dl>
    );
}

function WorkOrderDetailPage() {
    const id = Number(useParams().workOrderId);
    const { data, error, loading } = useApi(() => workOrdersApi.get(id), [id]);
    const [updated, setWorkOrder] = useState<WorkOrder>();
    const workOrder = updated?.id === id ? updated : data;

    return (
        <>
            <Link className="back-link" to={ROUTES.workOrders}>← All work orders</Link>
            <StateMessage loading={loading} error={error} />
            {workOrder && (
                <>
                    <PageHeader title={`${workOrder.code} · ${workOrder.title}`} subtitle={`${workOrder.siteName}, ${workOrder.countryCode}`} />
                    <Card title="Details"><WorkOrderDetails wo={workOrder} /></Card>
                    <Card title="Actions"><WorkOrderStatusActions key={workOrder.status} workOrder={workOrder} onChanged={setWorkOrder} /></Card>
                </>
            )}
        </>
    );
}

export default WorkOrderDetailPage;
