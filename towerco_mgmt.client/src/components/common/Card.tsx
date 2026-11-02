import type { ReactNode } from 'react';

interface CardProps {
    title?: string;
    subtitle?: string;
    actions?: ReactNode;
    children: ReactNode;
}

function CardHeader({ title, subtitle, actions }: Omit<CardProps, 'children'>) {
    return (
        <header className="card-header">
            <div>
                {title && <h2>{title}</h2>}
                {subtitle && <p className="card-subtitle">{subtitle}</p>}
            </div>
            {actions && <div className="card-actions">{actions}</div>}
        </header>
    );
}

function Card({ title, subtitle, actions, children }: CardProps) {
    const hasHeader = !!(title || subtitle || actions);
    return (
        <section className="card">
            {hasHeader && <CardHeader title={title} subtitle={subtitle} actions={actions} />}
            {children}
        </section>
    );
}

export default Card;
