import type { HealthFilter, PortfolioHealth, SiteHealth } from '@/features/health/types';

export interface HealthApi {
    portfolio: (filter?: HealthFilter) => Promise<PortfolioHealth>;
    site: (siteId: number) => Promise<SiteHealth>;
}
