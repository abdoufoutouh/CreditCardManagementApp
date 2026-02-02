<template>
  <div class="topbar">
    <div class="topbar-left">
      <h1 class="page-title">{{ title }}</h1>
    </div>
    
    <div class="topbar-right">
      <Button 
        variant="ghost" 
        size="small" 
        @click="handleLogout"
        :loading="loading"
        class="logout-btn"
      >
        Logout
      </Button>
      <div class="user-info">
        <div class="user-avatar">
          <div class="avatar-circle"></div>
        </div>
        <span class="username">{{ username }}</span>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import { useAuthStore } from '../../store/authStore';
import Button from '../common/Button.vue';

const props = defineProps({
  title: {
    type: String,
    default: 'Dashboard'
  },
  username: {
    type: String,
    default: 'John Doe'
  }
});

const router = useRouter();
const authStore = useAuthStore();
const loading = ref(false);

const handleLogout = async () => {
  loading.value = true;
  try {
    await authStore.logout();
    router.push('/login');
  } catch (error) {
    console.error('Logout failed:', error);
  } finally {
    loading.value = false;
  }
};
</script>

<style scoped>
.topbar {
  height: 70px;
  background: white;
  border-bottom: 1px solid #e8e8e8;
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 0 30px;
  position: fixed;
  top: 0;
  right: 0;
  left: 260px;
  z-index: 100;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.05);
}

.topbar-left {
  flex: 1;
}

.page-title {
  margin: 0;
  font-size: 24px;
  font-weight: 600;
  color: #2c3e50;
}

.topbar-right {
  display: flex;
  align-items: center;
  gap: 15px;
}

.logout-btn {
  margin-right: 10px;
}

.user-info {
  display: flex;
  align-items: center;
  gap: 12px;
}

.user-avatar {
  position: relative;
}

.avatar-circle {
  width: 40px;
  height: 40px;
  border-radius: 50%;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  border: 2px solid #fff;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.15);
}

.username {
  font-size: 14px;
  font-weight: 500;
  color: #2c3e50;
}
</style>
