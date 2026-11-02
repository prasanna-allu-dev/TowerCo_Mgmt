import type { PortfolioDashboard } from '@/features/dashboard/types';

export interface DashboardApi {
    get: () => Promise<PortfolioDashboard>;
}
