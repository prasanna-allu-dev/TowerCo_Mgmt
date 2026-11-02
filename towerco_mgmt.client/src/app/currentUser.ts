/** Signed-in user shown in the top bar. Auth is out of scope for this demo, so the user is fixed. */
export const CURRENT_USER = {
    name: 'Demo User',
    photoUrl: '/images/user-photo.jpg',
} as const;

const INITIALS_LENGTH = 2;

export const toInitials = (name: string) =>
    name.split(/\s+/).filter(Boolean).slice(0, INITIALS_LENGTH).map(part => part[0].toUpperCase()).join('');
