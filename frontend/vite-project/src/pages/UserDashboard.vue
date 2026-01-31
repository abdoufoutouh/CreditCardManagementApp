<template>
  <div class="dashboard-container">
    <div class="dashboard-card">
      <h1>User Dashboard</h1>
      <div class="success-message">
        <p>You are successfully connected</p>
      </div>
      <div v-if="authStore.user" class="user-info">
        <p><strong>Name:</strong> {{ authStore.user.firstName }} {{ authStore.user.lastName }}</p>
        <p><strong>Email:</strong> {{ authStore.user.email }}</p>
      </div>
      <button @click="handleLogout" class="logout-btn">Logout</button>
    </div>
  </div>
</template>

<script setup>
import { useRouter } from 'vue-router';
import { useAuthStore } from '../store/authStore';

const router = useRouter();
const authStore = useAuthStore();

const handleLogout = async () => {
  await authStore.logout();
  router.push('/login');
};
</script>

<style scoped>
.dashboard-container {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: 100vh;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  padding: 20px;
}

.dashboard-card {
  background: white;
  border-radius: 12px;
  padding: 40px;
  width: 100%;
  max-width: 500px;
  box-shadow: 0 10px 25px rgba(0, 0, 0, 0.2);
  text-align: center;
}

h1 {
  margin: 0 0 30px 0;
  color: #333;
  font-size: 32px;
}

.success-message {
  background: #d4edda;
  border: 1px solid #c3e6cb;
  border-radius: 8px;
  padding: 20px;
  margin-bottom: 30px;
}

.success-message p {
  margin: 0;
  color: #155724;
  font-size: 18px;
  font-weight: 500;
}

.user-info {
  text-align: left;
  background: #f8f9fa;
  border-radius: 8px;
  padding: 20px;
  margin-bottom: 30px;
}

.user-info p {
  margin: 10px 0;
  color: #555;
}

.logout-btn {
  width: 100%;
  padding: 12px;
  background: #e74c3c;
  color: white;
  border: none;
  border-radius: 6px;
  font-size: 16px;
  font-weight: 600;
  cursor: pointer;
  transition: background 0.3s;
}

.logout-btn:hover {
  background: #c0392b;
}
</style>
