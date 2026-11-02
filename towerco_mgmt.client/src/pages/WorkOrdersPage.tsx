import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { workOrdersApi } from '@/api';
import { Card, StateMessage } from '@/components/common';
import { CreateWorkOrderForm, WorkOrderFilters, WorkOrderKpis, WorkOrdersTable } from '@/components/workOrders';
import '@/components/workOrders/workOrders.css';
import { workOrderPath } from '@/constants/navigation';
import { useApi } from '@/hooks/useApi';
import PageHeader from '@/layout/PageHeader';
import type { WorkOrder, WorkOrderFilter } from '@/types';

function WorkOrdersPage() {
    const navigate = useNavigate();
    const [filter, setFilter] = useState<WorkOrderFilter>({});
    const [creating, setCreating] = useState(false);
    const { data, error, loading } = useApi(() => workOrdersApi.list(filter), [filter.search, filter.status, filter.priority, filter.country]);
    const hasRows = !loading && !error && !!data?.length;
    const onCreated = (wo: WorkOrder) => navigate(workOrderPath(wo.id));
    const newButton = !creating && <button className="wo-btn primary" onClick={() => setCreating(true)}>New work order</button>;

    return (
        <>
            <PageHeader title="Work orders" subtitle="Plan, assign and track maintenance and installation work on sites" actions={newButton} />
            {creating && <Card title="New work order"><CreateWorkOrderForm onCreated={onCreated} onCancel={() => setCreating(false)} /></Card>}
            {data && <WorkOrderKpis workOrders={data} />}
            <Card><WorkOrderFilters value={filter} onChange={setFilter} /></Card>
            <Card>
                <StateMessage loading={loading} error={error} empty={data?.length === 0} emptyText="No work orders match the filters." />
                {hasRows && <WorkOrdersTable workOrders={data} />}
            </Card>
        </>
    );
}

export default WorkOrdersPage;
