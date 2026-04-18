#!/bin/bash
# ============================================================
# Restaurant System - Deployment Script
# ============================================================

set -e

# الألوان
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# الدوال
print_header() {
    echo -e "${GREEN}========================================${NC}"
    echo -e "${GREEN}$1${NC}"
    echo -e "${GREEN}========================================${NC}"
}

print_error() {
    echo -e "${RED}❌ خطأ: $1${NC}"
}

print_success() {
    echo -e "${GREEN}✅ $1${NC}"
}

print_warning() {
    echo -e "${YELLOW}⚠️  تحذير: $1${NC}"
}

# الفحوصات
check_docker() {
    print_header "🐳 فحص Docker"
    
    if ! command -v docker &> /dev/null; then
        print_error "Docker غير مثبت!"
        exit 1
    fi
    print_success "Docker مثبت: $(docker --version)"
    
    if ! command -v docker-compose &> /dev/null; then
        print_error "Docker Compose غير مثبت!"
        exit 1
    fi
    print_success "Docker Compose مثبت: $(docker-compose --version)"
}

check_env_files() {
    print_header "📝 فحص ملفات البيئة"
    
    if [ ! -f ".env" ]; then
        print_error ".env file غير موجود!"
        print_warning "نسخ من .env.example..."
        cp .env.example .env
        print_warning "الرجاء تعديل .env بقيمك الفعلية وإعادة التشغيل"
        exit 1
    fi
    print_success ".env موجود"
}

build_image() {
    local env=$1
    
    if [ "$env" == "prod" ]; then
        print_header "🔨 بناء صورة Production"
        docker-compose -f compose.prod.yaml build
    else
        print_header "🔨 بناء صورة Development"
        docker-compose build
    fi
    
    print_success "البناء مكتمل"
}

start_services() {
    local env=$1
    
    print_header "🚀 بدء الخدمات"
    
    if [ "$env" == "prod" ]; then
        docker-compose -f compose.prod.yaml --env-file .env.prod up -d
    else
        docker-compose up -d
    fi
    
    print_success "الخدمات قيد التشغيل"
}

wait_for_services() {
    print_header "⏳ انتظار بدء الخدمات"
    
    sleep 5
    
    # انتظر قاعدة البيانات
    local max_attempts=30
    local attempt=1
    
    while [ $attempt -le $max_attempts ]; do
        if docker exec restaurant_api curl -f http://localhost:5183/swagger/index.html > /dev/null 2>&1; then
            print_success "API جاهز!"
            return 0
        fi
        
        echo "محاولة $attempt من $max_attempts..."
        sleep 2
        ((attempt++))
    done
    
    print_error "انتهت المهلة الزمنية - لم يتمكن API من البدء"
    return 1
}

show_status() {
    print_header "📊 حالة الخدمات"
    docker ps --filter "label!=.docker.compose.service" --format "table {{.Names}}\t{{.Status}}"
}

show_logs() {
    print_header "📋 السجلات"
    echo "عرض سجلات API (آخر 20 سطر):"
    docker logs --tail 20 restaurant_api
}

cleanup() {
    print_header "🧹 تنظيف"
    docker system prune -f
    print_success "تم التنظيف"
}

# القائمة الرئيسية
main() {
    echo ""
    echo "🍽️  Restaurant System - أداة النشر"
    echo ""
    echo "اختر الإجراء:"
    echo "1) نشر Development (مع قاعدة بيانات محلية)"
    echo "2) نشر Production (مع قاعدة بيانات خارجية)"
    echo "3) إيقاف الخدمات"
    echo "4) عرض الحالة"
    echo "5) عرض السجلات"
    echo "6) تنظيف الموارد"
    echo "0) خروج"
    echo ""
    read -p "اختيارك: " choice
    
    case $choice in
        1)
            check_docker
            check_env_files
            build_image "dev"
            start_services "dev"
            wait_for_services
            show_status
            echo ""
            print_success "تم نشر Development بنجاح!"
            echo "API متاح على: http://localhost:5183"
            ;;
        2)
            check_docker
            if [ ! -f ".env.prod" ]; then
                print_error ".env.prod غير موجود!"
                cp .env.prod.example .env.prod
                print_warning "الرجاء تعديل .env.prod بقيمك الفعلية"
                exit 1
            fi
            build_image "prod"
            start_services "prod"
            wait_for_services
            show_status
            echo ""
            print_success "تم نشر Production بنجاح!"
            ;;
        3)
            print_header "إيقاف الخدمات"
            docker-compose down
            print_success "تم إيقاف جميع الخدمات"
            ;;
        4)
            show_status
            ;;
        5)
            show_logs
            ;;
        6)
            cleanup
            ;;
        0)
            echo "وداعاً! 👋"
            exit 0
            ;;
        *)
            print_error "اختيار غير صحيح"
            exit 1
            ;;
    esac
}

# تشغيل البرنامج الرئيسي
main
