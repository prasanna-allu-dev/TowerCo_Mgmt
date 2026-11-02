import { useState } from 'react';
import { equipmentApi } from '@/features/equipment/api';
import { Card, StateMessage } from '@/shared/components';
import { EquipmentFilters, EquipmentTable, EquipmentTypesTable } from '@/features/equipment/components';
import { useApi } from '@/shared/hooks/useApi';
import PageHeader from '@/shared/components/PageHeader';
import type { EquipmentFilter } from '@/features/equipment/types';

function EquipmentPage() {
    const [filter, setFilter] = useState<EquipmentFilter>({});
    const items = useApi(() => equipmentApi.list(filter), [filter.search, filter.category, filter.status, filter.country]);
    const types = useApi(() => equipmentApi.types());
    const hasRows = !items.loading && !items.error && !!items.data?.length;

    return (
        <>
            <PageHeader title="Equipment" subtitle="Tenant-owned equipment across all towers" />
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
