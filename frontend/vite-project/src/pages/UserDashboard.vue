<template>
  <DashboardLayout title="Dashboard" username="John Doe">
    <div class="dashboard-content">
      <WelcomeSection />
      
      <StatsSection :stats="stats" />
      
      <CardsSection :cards="creditCards" />
      
      <ActivitySection />
    </div>
  </DashboardLayout>
</template>

<script setup>
import { ref } from 'vue';
import DashboardLayout from '../components/layout/DashboardLayout.vue';
import WelcomeSection from '../components/sections/WelcomeSection.vue';
import StatsSection from '../components/sections/StatsSection.vue';
import CardsSection from '../components/sections/CardsSection.vue';
import ActivitySection from '../components/sections/ActivitySection.vue';

const stats = ref([
  {
    id: 1,
    title: 'Total Cards',
    value: '3',
    icon: '💳',
    iconColor: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)'
  },
  {
    id: 2,
    title: 'Active Cards',
    value: '2',
    icon: '✅',
    iconColor: 'linear-gradient(135deg, #4CAF50 0%, #45a049 100%)'
  },
  {
    id: 3,
    title: 'Total Balance',
    value: '$12,450',
    icon: '💰',
    iconColor: 'linear-gradient(135deg, #FF6B6B 0%, #FF8E53 100%)'
  },
  {
    id: 4,
    title: 'Last Activity',
    value: '2h ago',
    icon: '🕐',
    iconColor: 'linear-gradient(135deg, #4ECDC4 0%, #44A08D 100%)'
  }
]);

const creditCards = ref([
  {
    id: 1,
    cardType: 'Visa',
    maskedNumber: ['****', '****', '****', '1234'],
    cardholderName: 'John Doe',
    expirationDate: '12/25'
  },
  {
    id: 2,
    cardType: 'MasterCard',
    maskedNumber: ['****', '****', '****', '5678'],
    cardholderName: 'John Doe',
    expirationDate: '09/24'
  }
]);
</script>

<style scoped>
.dashboard-content {
  display: flex;
  flex-direction: column;
  gap: 30px;
}
</style>
