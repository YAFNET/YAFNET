BootstrapEmail.configure do |config|
   # Defaults to ./bootstrap-email.scss or ./app/assets/stylesheets/bootstrap-email.scss in rails
  config.sass_email_location = File.expand_path('bootstrap-email.scss', __dir__)
end