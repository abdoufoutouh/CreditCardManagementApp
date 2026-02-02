import { createRouter, createWebHistory } from 'vue-router';
import { useAuthStore } from '../store/authStore';
import Login from '../pages/Login.vue';
import Signup from '../pages/Signup.vue';
import UserDashboard from '../pages/UserDashboard.vue';
import CreateCreditCard from '../pages/creditcards/CreateCreditCard.vue';

const routes = [
    { 
        path: '/', 
        redirect: '/login'
    },
    { 
        path: '/login', 
        component: Login,
        meta: { requiresGuest: true }
    },
    { 
        path: '/signup', 
        component: Signup,
        meta: { requiresGuest: true }
    },
    { 
        path: '/dashboard', 
        component: UserDashboard,
        meta: { requiresAuth: true }
    },
    { 
        path: '/creditcards/create', 
        component: CreateCreditCard,
        meta: { requiresAuth: true }
    }
];

const router = createRouter({
    history: createWebHistory(),
    routes
});

// Route guard for authentication
router.beforeEach((to, from, next) => {
    const authStore = useAuthStore();
    const requiresAuth = to.matched.some(record => record.meta.requiresAuth);
    const requiresGuest = to.matched.some(record => record.meta.requiresGuest);

    if (requiresAuth && !authStore.isAuthenticated) {
        next('/login');
    } else if (requiresGuest && authStore.isAuthenticated) {
        next('/dashboard');
    } else {
        next();
    }
});

export default router;
