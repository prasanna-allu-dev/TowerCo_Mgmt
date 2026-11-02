import { useState } from 'react';
import { operatorsApi, tenantsApi } from '@/api';
import { Card, StateMessage } from '@/components/common';
import { TenantFilters, TenantKpis, TenantsTable } from '@/components/tenants';
import { useApi } from '@/hooks/useApi';
import PageHeader from '@/layout/PageHeader';
import type { TenantFilter } from '@/types';

function TenantsPage() {
    const [filter, setFilter] = useState<TenantFilter>({});
    const operators = useApi(() => operatorsApi.list());
    const { data, error, loading } = useApi(() => tenantsApi.list(filter), [filter.search, filter.operatorId, filter.country, filter.state]);
    const hasRows = !loading && !error && !!data?.length;

    return (
        <>
            <PageHeader title="Tenants" subtitle="Operator tenancies across the portfolio and the equipment they own" />
            {data && <TenantKpis tenants={data} />}
            <Card><TenantFilters value={filter} operators={operators.data ?? []} onChange={setFilter} /></Card>
            <Card>
                <StateMessage loading={loading} error={error} empty={data?.length === 0} emptyText="No tenancies match the filters." />
                {hasRows && <TenantsTable tenants={data} />}
            </Card>
        </>
    );
}

export default TenantsPage;
