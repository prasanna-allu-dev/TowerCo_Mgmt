import { useState } from 'react';
import { equipmentApi } from '@/api';
import { Card, StateMessage } from '@/components/common';
import { EquipmentFilters, EquipmentTable, EquipmentTypesTable } from '@/components/equipment';
import { useApi } from '@/hooks/useApi';
import PageHeader from '@/layout/PageHeader';
import type { EquipmentFilter } from '@/types';

function EquipmentPage() {
    const [filter, setFilter] = useState<EquipmentFilter>({});
    const items = useApi(() => equipmentApi.list(filter), [filter.search, filter.category, filter.owner, filter.status, filter.country]);
    const types = useApi(() => equipmentApi.types());
    const hasRows = !items.loading && !items.error && !!items.data?.length;

    return (
        <>
            <PageHeader title="Equipment" subtitle="Tenant-owned and TowerCo shared equipment across all towers" />
            <Card><EquipmentFilters value={filter} onChange={setFilter} /></Card>
            <Card title="Equipment register">
                <StateMessage loading={items.loading} error={items.error} empty={items.data?.length === 0} emptyText="No equipment matches the filters." />
                {hasRows && <EquipmentTable items={items.data!} showSite />}
            </Card>
            <Card title="Equipment catalogue">
                <StateMessage loading={types.loading} error={types.error} />
                {types.data && <EquipmentTypesTable types={types.data} />}
            </Card>
        </>
    );
}

export default EquipmentPage;
